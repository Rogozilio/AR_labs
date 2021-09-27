using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject _car;
    private GameObject _maskCar;
    private Vector3 _rayStartPosition;
    private PlayerInput _input;

    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _input.actions["Touch"].started += ctx => CreateCar(ctx);

        _rayStartPosition = new Vector3(Screen.height / 2, Screen.width / 2, 0);
        _car = GameObject.FindGameObjectWithTag("Player");
        _maskCar = GameObject.FindGameObjectWithTag("MaskCar");

        _car.SetActive(false);
        _maskCar.SetActive(false);
    }

    void Update()
    {
        CreateMaskCar();
    }

    private void CreateMaskCar()
    {
        Ray ray = Camera.main.ScreenPointToRay(_rayStartPosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        if (_maskCar != null)
        {
            if (hit.collider != null)
            {
                _maskCar.SetActive(true);
                _maskCar.transform.position = hit.point;
            }
            else
            {
                _maskCar.SetActive(false);
            }
        }
    }

    private void CreateCar(InputAction.CallbackContext context)
    {
        if (_maskCar != null && _maskCar.activeSelf)
        {
            _car.SetActive(true);
            _car.transform.position = _maskCar.transform.position;
            Destroy(_maskCar);
        }
    }
}