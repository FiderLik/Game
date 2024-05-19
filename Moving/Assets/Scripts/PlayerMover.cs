using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Новая библиотека
public class PlayerMover : MonoBehaviour
{
    // Поле для скорости передвижения
    public float moveSpeed;
    // Направление передвижения
    public Transform orientation;
    // Поле для хранения физического компонента
    private Rigidbody _rb;
    // Поле для хранения системы ввода
    private PlayerInput _input;
    // Поле для хранения слоя
    public LayerMask whatIsGround;
    // Логическое поле для определения земли под ногами 
    bool _grounded;

    public AudioSource _audioSource;


    private void Awake()
    {
        // Создаем объект PlayerInput
        _input = new PlayerInput();
        // Вызываем прыжок
        _input.Player.Jump.performed += context => Jump();
    }
    private void Start()
    {
        // Тут мы ищем физический компонент
        _rb = GetComponent<Rigidbody>();
        // Отключение автоповорота
        _rb.freezeRotation = true;
    }
    private void Update()
    {
        // Проверка на землю под ногами
        _grounded = Physics.Raycast(transform.position, Vector3.down, 1.5f, whatIsGround);
        // Вызов метода для перемещения
        Move();
        // Вызов метода для контроля скорости
        SpeedControl();
        // Изменения трения в зависимости ситуации (земля - воздух)
        if (_grounded)
        {
            _rb.drag = 0.5f;
        }
        else
        {
            _rb.drag = 0;
        }
    }
    private void Move()
    {
        // Считываем вектор, полученный от нажатия на клавишу
        Vector2 _direction = 
            _input.Player.Move.ReadValue<Vector2>();
        if (_direction != Vector2.zero)
        {
            //проигрываем новый звук, только если сейчас никакой звук не играет
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            _audioSource.Stop();
        }


        // Получаем вектор в направлении ориентира
        Vector3 moveDirection = 
            orientation.right * _direction.x 
            + orientation.forward*_direction.y;
        // Добавляем ускорение нашему объекту через силу!
        _rb.AddForce(moveDirection.normalized * moveSpeed, 
                        ForceMode.Force);
    }
    private void SpeedControl()
    {
        // Создадим вектор для стабилизации скорости
        Vector3 _flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        // ограничиваем скорость, если вдруг разогнались
        if (_flatVel.magnitude > moveSpeed)
        {
            // Нормализуй вектор, прежде чем умножить
            Vector3 _limitedVel = _flatVel.normalized * moveSpeed;
            _rb.velocity = new Vector3(_limitedVel.x, _rb.velocity.y, _limitedVel.z);
        }
    }
    // Прыгаем
    private void Jump()
    {
        // Проверяем, на земле ли мы
        if (_grounded)
        {
            // Обнуляем ускорение по вертикали
            _rb.velocity = 
                new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    } 
    // Включаем систему вводв
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
