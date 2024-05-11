using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_shokudomae : MonoBehaviour
{
    float x, z;
    float xRot, yRot;
    public float speed = 0.05f;

    public GameObject cam;
    Quaternion cameraRot, characterRot;
    public float Xsensitivity = 3f, Ysensitivity = 3f;
    
    bool cursorLock = true;

    //変数の宣言(角度の制限用)
    float minX = -90f, maxX = 90f;

    // Start is called before the first frame update
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        xRot = 0;
        yRot = 0;

        xRot = Input.GetAxis("Mouse X") * Xsensitivity;
        yRot = Input.GetAxis("Mouse Y") * Ysensitivity;

        if(Input.GetKey(KeyCode.UpArrow))
            yRot = Ysensitivity;
        if(Input.GetKey(KeyCode.LeftArrow))
            xRot = -Xsensitivity;
        if(Input.GetKey(KeyCode.DownArrow))
            yRot = -Ysensitivity;
        if(Input.GetKey(KeyCode.RightArrow))
            xRot = Xsensitivity;

        // float xRot = Input.GetAxis("Mouse X") * Xsensitivity;
        // float yRot = Input.GetAxis("Mouse Y") * Ysensitivity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        //Updateの中で作成した関数を呼ぶ
        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;


        UpdateCursorLock();
    }

    private void FixedUpdate()
    {
        x = 0;
        z = 0;

        if(Input.GetKey(KeyCode.W))
            z = speed;
        if(Input.GetKey(KeyCode.A))
            x = -speed;
        if(Input.GetKey(KeyCode.S))
            z = -speed;
        if(Input.GetKey(KeyCode.D))
            x = speed;

        // x = Input.GetAxisRaw("Horizontal") * speed;
        // z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x,0,z);

        transform.position += cam.transform.forward * z + cam.transform.right * x;
    }


    public void UpdateCursorLock()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if(Input.GetMouseButton(0))
        {
            cursorLock = true;
        }


        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    //角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX,minX,maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }


}

// public class PlayerController : MonoBehaviour
// {
//     private Rigidbody _rigidbody;
//     private Transform _transform;
//     private Animator _animator;
//     private float _horizontal;
//     private float _vertical;
//     private Vector3 _velocity;
//     private float _speed = 2f;

//     private Vector3 _aim; // 追記
//     private Quaternion _playerRotation; // 追記

//     void Start()
//     {
//         _rigidbody = GetComponent<Rigidbody>();
//         _transform = GetComponent<Transform>();
//         _animator = GetComponent<Animator>();

//         _playerRotation = _transform.rotation; // 追記

//     }

//     void FixedUpdate()
//     {
//         _horizontal = Input.GetAxis("Horizontal");
//         _vertical = Input.GetAxis("Vertical");

//         var _horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up); // 追記

//         _velocity = _horizontalRotation * new Vector3(_horizontal, _rigidbody.velocity.y, _vertical).normalized; // 修正

//         _aim = _horizontalRotation * new Vector3(_horizontal, 0, _vertical).normalized; // 追記

//         if(_aim.magnitude > 0.5f) { // 以下追記
//             _playerRotation = Quaternion.LookRotation(_aim, Vector3.up);
//         }

//         _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _playerRotation, 600 * Time.deltaTime); // 追記

//         if (_velocity.magnitude > 0.1f) {
//             _animator.SetBool("walking", true);
//         } else {
//             _animator.SetBool("walking", false);
//         }

//         _rigidbody.velocity = _velocity * _speed;
//     }
// }
