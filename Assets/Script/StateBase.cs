using UnityEngine;

public abstract class StateBase
{
    public virtual void EnterState(Ring r) { }
    public virtual void ExitState(Ring r) { }
    public abstract StateBase Update(Ring r);
}

public class State
{
    protected Ring _ring;
    private MoveState _move;

    public IdleState idle;
    public DragState dragged;
    public DropState dropped;
    
    public MoveState Move { get { return _move; } private set { } }
    
    public State(Ring ring)
    {

        dragged = new DragState();
        dropped = new DropState();
        idle = new IdleState();
        
        _ring = ring;
        _move = idle;
        _move.EnterState(_ring);
    }
    
    /** Manage the ring's state update
     * Handling the "Input state changers" before the "Update state changers"
     */
    public void Update()
    {
        StateBase state = _move.Update(_ring);
        if (state != null)
        {
            ChangeMoveState((MoveState)state);
        }

    }
    
    public void ChangeMoveState(MoveState next)
    {
        if (_move == null || _move == next) return;
        _move.ExitState(_ring);
        _move = next;
        _move.EnterState(_ring);
    }

    public void OnTriggerEnter2D(Ring r, Collider2D col)
    {
        _move.OnTriggerEnter2D(r, col);
    }
    public void OnTriggerExit2D(Ring r, Collider2D col)
    {
        _move.OnTriggerExit2D(r, col);
    }

    public void OnCollisionEnter2D(Ring r, Collision2D col)
    {
        _move.OnCollisionEnter2D(r, col);
    }
    public void OnCollisionExit2D(Ring r, Collision2D col)
    {
        _move.OnCollisionExit2D(r, col);
    }

}

public abstract class MoveState : StateBase
{
    protected float _initialYPos = 0f;

    public override StateBase Update(Ring ring)
    {
        return this;
    }

    public override void EnterState(Ring ring) { }
    public virtual void OnTriggerEnter2D(Ring r, Collider2D col) {}
    public virtual void OnTriggerExit2D(Ring r, Collider2D col) {}
    public virtual void OnCollisionEnter2D(Ring r, Collision2D col) { }
    public virtual void OnCollisionExit2D(Ring r, Collision2D col) { }
}

[System.Serializable]
public class IdleState : MoveState
{
    public override StateBase Update(Ring ring)
    {
        return this;
    }
    public override void EnterState(Ring ring)
    {
        if (ring.CurrentPin)
        {
            ring.CurrentPin.AddRing(ring);
        }
    }
    public override void ExitState(Ring ring) { }

}

[System.Serializable]
public class DragState : MoveState
{
    private bool bIsColliding;
    private bool bIsTriggered;

    public override StateBase Update(Ring ring)
    {
        if(!ring.bInMyHome && !bIsColliding && !bIsTriggered) ring.transform.rotation = Quaternion.Lerp(ring.transform.rotation, Quaternion.Euler(ring.transform.rotation.x, ring.transform.rotation.y, 0f), Time.deltaTime * 5);
        return this;
    }
    public override void EnterState(Ring ring) { }
    public override void ExitState(Ring ring) { }

    public override void OnTriggerEnter2D(Ring r, Collider2D col)
    {
        base.OnTriggerEnter2D(r, col);
        bIsTriggered = true;
        Pin p = col.GetComponentInParent<Pin>();
        if (p && r.CurrentPin != p && p.gameObject.tag == Pin.PinTagName)
        {
            if (r.CurrentPin)
            {
                r.TriggerPin = p;
            }
            else
            {
                r.CurrentPin = p;
            }
        }
    }

    public override void OnTriggerExit2D(Ring r, Collider2D col)
    {
        base.OnTriggerExit2D(r, col);
        bIsTriggered = false;
    }

    public override void OnCollisionEnter2D(Ring r, Collision2D col)
    {
        bIsColliding = true;
    }
    public override void OnCollisionExit2D(Ring r, Collision2D col)
    {
        bIsColliding = false;
    }

}

[System.Serializable]
public class DropState : MoveState
{
    public override StateBase Update(Ring ring)
    {
        return this;
    }
    public override void EnterState(Ring ring) { }
    public override void ExitState(Ring ring) { }

    public override void OnTriggerEnter2D(Ring r, Collider2D col)
    {
        Pin p = col.GetComponentInParent<Pin>();
        if (p && r.CurrentPin != p && p.gameObject.tag == Pin.PinTagName)
        {
            if (r.CurrentPin)
            {
                r.TriggerPin = p;
            }
            else
            {
                r.CurrentPin = p;
            }
        }
        base.OnTriggerEnter2D(r, col);
    }

    public override void OnTriggerExit2D(Ring r, Collider2D col)
    {
        base.OnTriggerExit2D(r, col);
    }
}
