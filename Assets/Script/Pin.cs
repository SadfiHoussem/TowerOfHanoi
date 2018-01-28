using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour {

    public Vector3 NextEmptySpot { get { return _nextEmptySpot; } set { _nextEmptySpot = value; } }
    public static List<Pin> Pins { get { return _pins; } private set { } }
    public static string PinTagName { get { return _PinTagName; } private set { } }
    
    private const string _PinTagName = "Pin";
    public Vector3 _nextEmptySpot = Vector3.zero;
    private List<Ring> rings;
    private static List<Pin> _pins;

    void Awake()
    {
        _pins = new List<Pin>();
        rings = new List<Ring>();
        _nextEmptySpot = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        foreach (GameObject go in GameObject.FindGameObjectsWithTag(PinTagName))
        {
            if (go.GetComponent<Pin>()) _pins.Add(go.GetComponent<Pin>());
        }
    }

    void Start() {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ring r = other.GetComponent<Ring>();
        
        // Check if the ring is at his correct pin
        if (r && !r.bInMyHome)
        {
            if (IsValidPin(r))
            {
                r.DestroyGroupSorting();
                AddRing(r);
                r.bInMyHome = true;
            }
            else
            {
                r.EjectRing();
            }
            

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Ring r = other.GetComponent<Ring>();

        if (r)
        {
            r.ResetRing();
            r.bInMyHome = false;
            r.AddGroupSorting();
            RemoveRing(r);
        }
    }

    public void AddRing(Ring r)
    {
        if (rings.Contains(r)) return;

        var index = rings.BinarySearch(r);

        if (index < 0) index = ~index;
        rings.Insert(index, r);
    }
    public void RemoveRing(Ring r)
    {
        rings.Remove(r);
        _nextEmptySpot.y -= r.GetComponent<BoxCollider2D>().size.y;
    }

    public bool IsValidPin(Ring r)
    {
        if (rings.Count == 0) return true;
        return rings[rings.Count - 1].Weight > r.Weight;
    }
    
}
