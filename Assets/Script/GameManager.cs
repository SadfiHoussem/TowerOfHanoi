using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public Pin StarterPin;
    public List<GameObject> RingsPrefabs;

    GameObject targetRing = null;
    Ring selectedRing;
    SpringJoint2D spring;

    #region Buttons
    private Command buttonReleasedLeftMouse, buttonPressedLeftMouse, buttonLeftMouse;
#endregion
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;
        
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        // Setup Resolution
        if ((UnityEngine.iOS.Device.generation.ToString()).IndexOf("iPhone5") > -1)
        {
            Screen.SetResolution(640, 1136, true);
        } else if ((UnityEngine.iOS.Device.generation.ToString()).IndexOf("iPadMini2Gen") > -1)
        {
            Screen.SetResolution(2048, 1536, true);
        }
        
    }

    void Start () {
        SpawnRingAtPin(StarterPin);

        buttonPressedLeftMouse = new SelectRingCommand();
        buttonLeftMouse = new DragRingCommand();
        buttonReleasedLeftMouse = new DropRingCommand();
    }
    
    class SelectRingCommand : Command
    {
        public override void Execute(GameManager gm)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), gm.transform.forward, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ring"));

            if (hit2D)
            {
                gm.selectedRing = hit2D.transform.gameObject.GetComponent<Ring>();
                if (gm.selectedRing && gm.selectedRing.bIsDraggable)
                {
                    gm.targetRing = hit2D.transform.gameObject;
                    gm.targetRing.GetComponent<Ring>().Dragged();

                    Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    
                    gm.spring = gm.targetRing.GetComponent<SpringJoint2D>();

                    // The anchor get's cursor's position
                    gm.spring.connectedAnchor = cursorPosition;
                    
                }
            }
            
        }
    }
    class DragRingCommand : Command
    {
        public override void Execute(GameManager gm)
        {
            
            if (!gm.spring || !gm.selectedRing.bIsDraggable) return;

            gm.spring.enabled = true;

            // Getting cursor position
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // The anchor get's cursor's position
            gm.spring.connectedAnchor = cursorPosition;

        }
    }

    class DropRingCommand : Command
    {
        public override void Execute(GameManager gm)
        {
            if (!gm.spring) return;

            // Disabling the spring component
            gm.spring.enabled = false;
            gm.spring = null;
            gm.selectedRing.bIsDraggable = true;
            gm.selectedRing.State.ChangeMoveState(gm.selectedRing.State.dropped);
            gm.selectedRing = null;
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buttonPressedLeftMouse.Execute(instance);
        }
        else if (Input.GetMouseButton(0) && targetRing)
        {
            buttonLeftMouse.Execute(instance);
        }
        else if (Input.GetMouseButtonUp(0) && targetRing)
        {
            buttonReleasedLeftMouse.Execute(instance);
        }
    }

	void Update () {

        HandleInput();
	}
    
    private void SpawnRingAtPin(Pin p)
    {
        RingsPrefabs.Sort((x, y) => y.GetComponent<Ring>().Weight.CompareTo(x.GetComponent<Ring>().Weight));
        foreach (GameObject go in RingsPrefabs)
        {
            Ring r = Instantiate(go, p.NextEmptySpot, Quaternion.identity).GetComponent<Ring>();
            p._nextEmptySpot.y += r.GetComponent<BoxCollider2D>().size.y;
            r.bInMyHome = true;
        }
    }

}
