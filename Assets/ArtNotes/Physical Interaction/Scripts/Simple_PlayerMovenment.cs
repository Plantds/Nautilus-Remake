using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_PlayerMovenment : MonoBehaviour
{
    [SerializeField] Rigidbody _body;
    [SerializeField] Vector3 _checkSpherePos;
    [SerializeField] float _checkSphereRadius = .2f, _jumpForce, _speed = 6;
    Transform _transformBody;

    void Start()
    {
        _transformBody = _body.transform;
        _body.freezeRotation = true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && Physics.CheckSphere(transform.position + _checkSpherePos, _checkSphereRadius))
            _body.AddForce(Vector3.up * _jumpForce);
    }

    void FixedUpdate()
    {
        Vector3 horizontalVelocity = Input.GetAxis("Vertical") * _transformBody.forward + Input.GetAxis("Horizontal") * _transformBody.right;
        horizontalVelocity *= _speed;
        _body.linearVelocity = horizontalVelocity + _transformBody.up * _body.linearVelocity.y;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + _checkSpherePos, _checkSphereRadius);
    }
}
