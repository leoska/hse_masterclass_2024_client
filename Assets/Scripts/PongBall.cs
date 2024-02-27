using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    [SerializeField] private float _initialSpeed = 10f;
    [SerializeField] private float _speedIncrease = 0.25f;
    [SerializeField] private float _waitTimeForStart = 2f;

    private int _hitCounter = 0;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartGame()
    {
        Invoke(nameof(StartBall), _waitTimeForStart);
    }

    private void FixedUpdate()
    {
        if (NetworkController.Type != PlayerType.Server)
            return;
        
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, _initialSpeed + _hitCounter * _speedIncrease);
    }

    private void StartBall()
    {
        rb.velocity = new Vector3(-_initialSpeed, 0f);
        _hitCounter = 0;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
    }
}
