using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;
    

    int matrixX;
    int matrixY;

    public bool attack = false;
    public bool isEnPassant = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game game = controller.GetComponent<Game>();

        if (attack)
        {
            if (isEnPassant)
            {

                GameObject captured = game.GetEnPassantTarget();
                if (captured != null)
                {
                    Chessman capturedCm = captured.GetComponent<Chessman>();

                    if (captured.name == "white_king") game.Winner("black");
                    if (captured.name == "black_king") game.Winner("white");

                    game.SetPositionEmptty(capturedCm.GetXBoard(), capturedCm.GetYBoard());
                    Destroy(captured);
                }
            }

            else
            {
                GameObject cp = game.GetPosition(matrixX, matrixY);
                if (cp.name == "white_king") game.Winner("black");
                if (cp.name == "black_king") game.Winner("white");
                Destroy(cp);
            }
        }

        Chessman movingCm = reference.GetComponent<Chessman>();
        int prevY = movingCm.GetYBoard();

        game.SetPositionEmptty(movingCm.GetXBoard(), movingCm.GetYBoard());

        movingCm.SetXBoard(matrixX);
        movingCm.SetYBoard(matrixY);
        movingCm.SetCoords();
        game.SetPosition(reference);


        if ((movingCm.name == "white_pawn" || movingCm.name == "black_pawn") && Mathf.Abs(matrixY - prevY) == 2)
        {
            game.SetEnPassantTarget(reference, matrixX, matrixY);
        }

        else
        {
            game.ClearEnPassantTarget();
        }

        game.NextTurn();
        reference.GetComponent<Chessman>().DestroyMovePlates();


        if (movingCm.name == "white_pawn" && matrixY == 7)
        {
            game.PromotePawn(reference);
            return;
        }

        if (movingCm.name == "black_pawn" && matrixY == 0)
        {
            game.PromotePawn(reference);
            return;
        }
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
    return reference;
    }
}

        //...SetPositionEmpty or SetPositionEmptty??? tt is ok but t is error
        controller.GetComponent<Game>().SetPositionEmptty(reference.GetComponent<Chessman>().GetXBoard(), reference.GetComponent<Chessman>().GetYBoard());

        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        controller.GetComponent<Game>().SetPosition(reference);

        controller.GetComponent<Game>().NextTurn();

        reference.GetComponent<Chessman>().DestroyMovePlates();

        Chessman cm = reference.GetComponent<Chessman>();

        if (cm.name == "white_pawn" && matrixY == 7)
        {
            controller.GetComponent<Game>().PromotePawn(reference);
            return;
        }

        if (cm.name == "black_pawn" && matrixY == 0)
        {
            controller.GetComponent<Game>().PromotePawn(reference);
            return;
        }
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
