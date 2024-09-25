using GameFrame;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    private Transform playerTrans;

    public void Register(Transform player)
    {
        playerTrans = player;
    }

    public void Reset()
    {
        transform.position = new Vector3(0, 0, -1);
    }

    public void ForceLookPlayer()
    {
        var position = playerTrans.position;
        transform.position = new Vector3(position.x, position.y, -10);   
    }
    
    private void LateUpdate()
    {
        if (playerTrans != null)
        {
            var position = playerTrans.position;
            transform.position = Vector3.Slerp(transform.position, new Vector3(position.x, position.y, -10), Time.deltaTime * 10);   
        }
    }
}
