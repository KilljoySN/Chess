using UnityEngine;

public class Roulette : MonoBehaviour
{
    public float RotatePower;
    public float StopPower;

    private Rigidbody2D rbody;
    int inRotate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
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

        if (rot > 0 && rot <= 45)
        {
            Win(200);
        }

        else if (rot > 45 && rot <= 90)
        {
            Win(300);
        }
        
        else if (rot > 90 && rot <= 135)
        {
            Win(300);
        }

        else if (rot > 135 && rot <= 180)
        {
            Win(300);
        }

        else if (rot > 180 && rot <= 225)
        {
            Win(300);
        }

        else if (rot > 225 && rot <= 270)
        {
            Win(300);
        }
        
        else if (rot > 270 && rot <= 315)
        {
            Win(300);
        }
        
        else if (rot > 315 && rot <= 360)
        {
            Win(300);
        }
    }

    public void Win(int Score)
    {
        print (Score);
    }
}
