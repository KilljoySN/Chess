using UnityEngine;

public class Roulette : MonoBehaviour
{
    public float RotatePower;
    public float StopPower;

    private Ridgidbody2D rbody;
    int inRotate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rbody = GetComponent<Ridgidbody2D>();
    }

    float t;

    // Update is called once per frame
    private void Update()
    {
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower*Time.deltaTime;

            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += 1*Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();

                inRotate = 0;
                t = 0;
            }
        }
    }

    private void Rotate()
    {
        if (inRotate == 0)
        {
            rbody.AddTorque(RotatePower);
            inRotate = 1;
        }
    }

    public void GetReward()
    {
        float rot = transform .eulerAngles.z;

        if (rot > 23 && rot <= 68f)
        {
            Win(200);
        }

        else if (rot > 68 && rot <= 113f)
        {
            Win(300);
        }
    }

    public void Win(int Score)
    {
        print (Score);
    }
}
