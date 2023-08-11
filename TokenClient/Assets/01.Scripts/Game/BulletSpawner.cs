using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private Transform _player;

    private float _spawnTime = 3f;
    private float _minSpawnTime = 0.25f;
    private float _speed = 2f;
    private float _maxSpeed = 20f;

    [SerializeField] private float _radius = 5;

    private void Start()
    {
        TimeController.Instance.TimeEvent += DecreaseTime;
        TimeController.Instance.TimeEvent += IncreaseTime;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (!TimeController.Instance.GameOver)
        {
            float x = Random.Range(-_radius, _radius);
            float y = Mathf.Sqrt(Mathf.Pow(_radius, 2) - Mathf.Pow(x, 2));
            y *= Random.Range(0, 2) == 0 ? -1 : 1;

            Vector2 pos = new Vector2(x, y);

            GameObject bullet = Instantiate(_bulletPrefab);
            bullet.transform.position = pos;
            bullet.GetComponent<Bullet>().Speed = _speed;
            bullet.GetComponent<Bullet>().Dir = ((Vector2)_player.position - pos).normalized;

            yield return new WaitForSeconds(_spawnTime);
        }
    }

    private void DecreaseTime(int time)
    {
        if (_spawnTime > _minSpawnTime)
        {
            _spawnTime -= 0.05f;
        }
    }

    private void IncreaseTime(int time)
    {
        if (_speed < _maxSpeed)
        {
            _speed += 0.1f;
        }
    }
}
