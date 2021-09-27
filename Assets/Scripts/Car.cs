using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    public float Speed;
    public float DelayShot;
    public GameObject WheelFL;
    public GameObject WheelFR;
    public GameObject WheelRL;
    public GameObject WheelRR;
    public GameObject Weapon;

    private PlayerInput _playerInput;
    private GameObject[] _projectiles;
    private GameObject[] _barrels;
    private GameObject _point;
    private Rigidbody _rigidbody;
    private Vector2 _dirMoveStick;
    private Vector2 _dirShotStick;
    private Vector3 _dirMove;
    private Vector3 _dirShot;
    private float _delayShot;

    private void Awake()
    {
        _delayShot = DelayShot;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        _barrels = GameObject.FindGameObjectsWithTag("Barrel");
        _point = GameObject.FindGameObjectWithTag("Respawn");
    }

    private void OnEnable()
    {
        foreach (var projectile in _projectiles)
        {
            projectile.SetActive(false);
        }

        foreach (var barrel in _barrels)
        {
            barrel.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        _dirMoveStick = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (_dirMoveStick != Vector2.zero)
        {
            _dirMove = new Vector3(_dirMoveStick.x, 0, _dirMoveStick.y);
            _dirMove = Camera.main.transform.TransformDirection(_dirMove);
            _dirMove = new Vector3(_dirMove.x, 0, _dirMove.z);
            Move();
        }
        else
        {
            ForwardWheeleDefault();
        }

        _dirShotStick = _playerInput.actions["Shot"].ReadValue<Vector2>();
        if (_dirShotStick != Vector2.zero)
        {
            _dirShot = new Vector3(_dirShotStick.x, 0, _dirShotStick.y);
            _dirShot = Camera.main.transform.TransformDirection(_dirShot);
            _dirShot = new Vector3(_dirShot.x, 0, _dirShot.z);
            Weapon.transform.forward = _dirShot.normalized;
            Shot();
        }

        CreateNewPoint();
    }


    private void Move()
    {
        Quaternion rotation = Quaternion.LookRotation(_dirMove);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * 5);
        _rigidbody.velocity = transform.forward * Speed;

        float angle = Vector3.Angle(_dirMove, transform.forward);
        float sign =
            Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(_dirMove, transform.forward)));
        float signed_angle = angle * sign;


        if (signed_angle > 5)
        {
            MoveLeft();
        }
        else if (signed_angle < -5)
        {
            MoveRight();
        }
        else
        {
            ForwardWheeleDefault();
        }

        MoveForward();
    }

    private void Shot()
    {
        if (_delayShot <= 0)
        {
            foreach (var projectile in _projectiles)
            {
                if (!projectile.activeSelf)
                {
                    projectile.SetActive(true);
                    projectile.transform.position
                        = Weapon.transform.position + Weapon.transform.forward / 30f;
                    projectile.transform.up
                        = Weapon.transform.forward;
                    break;
                }
            }

            _delayShot = DelayShot;
        }
        else
        {
            _delayShot -= Time.fixedDeltaTime;
        }
    }

    private void MoveForward()
    {
        WheelFL.transform.Rotate(Vector3.right, 30f);
        WheelFR.transform.Rotate(Vector3.right, 30f);
        WheelRL.transform.Rotate(Vector3.right, 30f);
        WheelRR.transform.Rotate(Vector3.right, 30f);
    }

    private void MoveLeft()
    {
        if ((int) WheelFL.transform.localEulerAngles.y != 150)
        {
            WheelFL.transform.localRotation
                = Quaternion.Euler(WheelFL.transform.localEulerAngles.x, -30, 0);
            WheelFR.transform.localRotation
                = Quaternion.Euler(WheelFR.transform.localEulerAngles.x, -30, 0);
        }
    }

    private void MoveRight()
    {
        if ((int) WheelFL.transform.localEulerAngles.y != 210)
        {
            WheelFL.transform.localRotation
                = Quaternion.Euler(WheelFL.transform.localEulerAngles.x, 30, 0);
            WheelFR.transform.localRotation
                = Quaternion.Euler(WheelFR.transform.localEulerAngles.x, 30, 0);
        }
    }

    private void ForwardWheeleDefault()
    {
        if ((int) WheelFL.transform.localEulerAngles.y != 180)
        {
            WheelFL.transform.localRotation
                = Quaternion.Euler(WheelFL.transform.localEulerAngles.x, 0, 0);
            WheelFR.transform.localRotation
                = Quaternion.Euler(WheelFR.transform.localEulerAngles.x, 0, 0);
        }
    }

    private Vector3 RandomPos(Vector3 center)
    {
        float value = 0.5f;
        return new Vector3(center.x + Random.Range(-value, value)
            , -0.4f, center.z + Random.Range(-value, value));
    }

    private void CreateNewPoint()
    {
        if (gameObject.activeSelf)
        {
            foreach (var barrel in _barrels)
            {
                if (barrel.activeSelf)
                {
                    return;
                }
            }

            if (!_point.activeSelf)
            {
                _point.SetActive(true);
                _point.transform.position = RandomPos(transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn"))
        {
            foreach (var barrel in _barrels)
            {
                barrel.SetActive(true);
                barrel.transform.position = RandomPos(other.transform.position);
            }

            other.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        _point.SetActive(false);
    }
}