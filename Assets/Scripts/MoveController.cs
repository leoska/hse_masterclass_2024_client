using System;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] private float _speed = 0.2f;
    [SerializeField] private Transform platformLeft;
    [SerializeField] private Transform platformRight;

    // Update is called once per frame
    void Update()
    {
        if (!NetworkController.isGameStarted)
            return;

        var key_w = Input.GetKey(KeyCode.W);
        var key_s = Input.GetKey(KeyCode.S);
        var move_y = Convert.ToInt32(key_w) - Convert.ToInt32(key_s);

        var _transform = NetworkController.Type == PlayerType.Server ? platformLeft : platformRight;

        var position = _transform.position;
        position = new Vector3(
            position.x,
            position.y + move_y,
            position.z
        );
        _transform.position = position;
    }
}
