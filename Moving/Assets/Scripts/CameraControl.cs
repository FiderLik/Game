using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Новая библиотека

public class CameraControl : MonoBehaviour
{
    // Поле для хранения системы ввода
    private PlayerInput _input;
    // Направление, которое хранит все возможные координаты объекта
    public Transform orientation;
    // Переменные текущего поворота
    float _xRotation;
    float _yRotation;
    private void Awake()
    {
        // Создаем объект PlayerInput
        _input = new PlayerInput();
    }
    private void Start()
    {
        // Блокируем Курсор и делаем его невидимым
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        // Считаем значение двумерного вектора
        Vector2 _cameraDirection = _input.Player.RotateCamera.ReadValue<Vector2>();
        // Изменяем повороты осей и ограничиваем значение от -90 до 90
        _yRotation += _cameraDirection.x;
        _xRotation -= _cameraDirection.y;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        // Поворот камеры
        gameObject.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        // Поворот ориентира
        orientation.rotation = Quaternion.Euler(0, _yRotation,0);
    }
    // Включаем систему ввода
    private void OnEnable()
    {
        _input.Enable();
    }
    // Выключаем систему ввода
    private void OnDisable()
    {
        _input.Disable();
    }
}
