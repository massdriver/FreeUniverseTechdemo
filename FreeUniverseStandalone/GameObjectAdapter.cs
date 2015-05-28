using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.World;

public interface IGameObjectDelegate
{
    void OnGUI();
    void OnUpdate();
    void OnCollisionEnter(Collision collision);
}

public interface ProjectileDelegate
{
    void OnTriggerEnter(Collider other);
    void OnCollisionEnter(Collision collision);
}

public class ProjectileBehaviour : MonoBehaviour
{
    ProjectileDelegate _projectileDelegate;
    public ProjectileDelegate projectileDelegate { get { return _projectileDelegate; } set { _projectileDelegate = value; } }
/*
    void OnCollisionEnter(Collision collision)
    {
        if (_projectileDelegate != null)
            _projectileDelegate.OnCollisionEnter(collision);
    }
    */
    void OnTriggerEnter(Collider other)
    {
        if (_projectileDelegate != null)
            _projectileDelegate.OnTriggerEnter(other);
    }
     
}

public class GameObjectUserData : MonoBehaviour
{
    object _userData;
    public object userData { get { return _userData; } set { _userData = value; } }
}

public class ObjectInspector : MonoBehaviour
{
    IObjectInspector _objInspector;
    public object objInspector { get { return _objInspector; } }
}

public class RayCastProvider : MonoBehaviour
{
    WorldRigidBody _worldRidigBody;
    public WorldRigidBody worldRidigBody { get { return _worldRidigBody; } set { _worldRidigBody = value; } }
}


public class GameObjectAdapter : MonoBehaviour
{
    private IGameObjectDelegate _delegate;

    public IGameObjectDelegate objectDelegate
    {
        get { return _delegate; }
        set { _delegate = value; }
    }

    //void OnGUI()
    //{
    //    if (null != _delegate)
    //       _delegate.OnGUI();
    //}

    //void Update()
    //{
    //    if (null != _delegate)
    //       _delegate.OnUpdate();
    //}

    //void OnCollisionEnter(Collision collision)
    //{
     //   if (null != _delegate)
    //      _delegate.OnCollisionEnter(collision);
    //}
}

public class BaseGameObject : IGameObjectDelegate
{
    protected GameObject _object;

    public BaseGameObject(bool doNotCreate)
    {
        if (doNotCreate == true)
            return;

        Create();
    }

    public BaseGameObject()
    {
        Create();
    }
    
    void Create()
    {
        _object = new GameObject();
        _object.AddComponent<GameObjectAdapter>().objectDelegate = this;
        _object.AddComponent<GameObjectUserData>();
    }

    public Transform transform { get { return _object.transform; } }

    public GameObject gameObject
    {
        get { return _object; }
    }

    public virtual void OnGUI()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnCollisionEnter(Collision collision)
    {

    }
}
