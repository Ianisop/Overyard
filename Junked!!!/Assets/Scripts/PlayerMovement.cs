using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 playerVelocity;
    public float playerSpeed = 200.0f;
    private float hoz, vert;
    public CharacterController _cc;


    void Update()
    {
        hoz = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");


        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        cameraForward *= vert;
        cameraRight *= hoz;

        Vector3 wantsMove = (cameraForward + cameraRight).normalized;

        _cc.Move(wantsMove * Time.deltaTime * playerSpeed);



    }
}