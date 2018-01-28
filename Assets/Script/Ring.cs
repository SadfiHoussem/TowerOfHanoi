using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System;

public class Ring : MonoBehaviour, IComparable<Ring>
{

    #region public fields
    public State State { get { return state; } private set { } }
    public Vector3 PosBeforeDrag { get { return _posBeforeDrag; } private set { } }
    public Pin CurrentPin { get { return _currentPin; } set { _currentPin = value; } }
    public Pin TriggerPin { get { return _triggerPin; } set { _triggerPin = value; } }
    [HideInInspector]
    public bool bIsDraggable;
    public int Weight;
    public float Speed;
    [HideInInspector]
    public bool bInMyHome;
    public BoxCollider2D bcRingLocker;
    #endregion

    #region private fields
    [SerializeField]
    private State state;
    private Vector3 _posBeforeDrag;
    private Rigidbody2D _ringRigidbody;
    private Pin _currentPin = null;
    private Pin _triggerPin;
    #endregion


    public int CompareTo(Ring other)
    {
        if (Weight == other.Weight) return 0;

        return Weight < other.Weight ? 1 : -1;
    }

    private void Awake()
    {
        state = new State(this);
    }

    void Start () {
        _ringRigidbody = GetComponent<Rigidbody2D>();
        Speed = 3.0f;
        bIsDraggable = true;
    }
    
    void Update () {
        state.Update();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        state.OnTriggerEnter2D(this, other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        state.OnTriggerExit2D(this, other);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        state.OnCollisionEnter2D(this, other);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        state.OnCollisionExit2D(this, other);
    }

    public void Dragged()
    {
        if (CurrentPin)
        {
            _posBeforeDrag = CurrentPin.NextEmptySpot;
        }
        else
        {
            _posBeforeDrag = transform.position;
        }
        state.ChangeMoveState(State.dragged);
    }

    public void Dropped()
    {
        if (_triggerPin && _triggerPin.IsValidPin(this))
        {
            CurrentPin.RemoveRing(this);
            CurrentPin = _triggerPin;
        }
        state.ChangeMoveState(State.dropped);
    }

    public void FreezeRing()
    {
        bIsDraggable = false;
    }

    public void UnfreezeRing()
    {
        bIsDraggable = true;
    }
    
    public void AddGroupSorting()
    {
        if (GetComponent<SortingGroup>() == null) gameObject.AddComponent<SortingGroup>().sortingOrder = 5;
    }

    public void DestroyGroupSorting()
    {
        Destroy(GetComponent<SortingGroup>());
    }

    public void EjectRing()
    {
        if (bcRingLocker)
        {
            bcRingLocker.enabled = true;
        }
    }
    public void ResetRing()
    {
        if (bcRingLocker)
        {
            bcRingLocker.enabled = false;
        }
    }
}
