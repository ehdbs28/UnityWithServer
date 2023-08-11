using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;

    private Rigidbody2D _rigid;

    private Vector2 _dir;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        InputDir();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void InputDir()
    {
        if (TimeController.Instance.GameOver)
        {
            _dir = Vector2.zero;
            return;
        }
        
        _dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Movement()
    {
        _rigid.velocity = _dir * _movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TimeController.Instance.GameOver = true;
            Destroy(other.gameObject);
            UIController.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}
