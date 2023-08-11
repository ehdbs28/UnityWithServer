using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed { get; set; }
    
    private Vector2 _dir;
    public Vector2 Dir
    {
        get => _dir;
        set
        {
            _dir = value;
            float angle = Mathf.Atan2(_dir.y, _dir.x);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GetComponent<Rigidbody2D>().velocity = _dir * Speed;
            Invoke("DistroySelf", 10f);
        }
    }

    private void DistroySelf()
    {
        Destroy(gameObject);
    }
}
