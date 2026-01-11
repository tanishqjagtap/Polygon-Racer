using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;
    public float height = 80f;
    public float smooth = 10f;
    public bool rotateWithCar = true;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = new Vector3(target.position.x, height, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);

        if (rotateWithCar)
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        else
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
