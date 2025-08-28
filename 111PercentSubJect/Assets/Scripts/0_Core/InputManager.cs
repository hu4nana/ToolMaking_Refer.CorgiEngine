using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector2 Move { get;private set; }
    public bool SkillPressed { get; private set; }

    private void Update()
    {
        Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //SkillPressed=Input.GetButtonDown()
    }

    public void Consume()
    {
        SkillPressed = false;
    }
}
