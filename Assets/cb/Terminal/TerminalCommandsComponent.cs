using UnityEngine;

class TerminalCommandsComponent : MonoBehaviour
{
    public Research Research;
    public Crafting Crafting;
    public Player Player;

    public Ball BlueBall;
    public Ball RedBall;
    public Ball YellowBall;

    void Start()
    {
        TerminalCommands.Component = this;
    }
}