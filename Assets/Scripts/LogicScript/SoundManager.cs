using UnityEngine;

public class SoundManager : Loader<SoundManager>
{
    [SerializeField] AudioClip arrow;
    [SerializeField] AudioClip rock;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip fireball;
    [SerializeField] AudioClip newGame;
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip hit;
    [SerializeField] AudioClip level;
    [SerializeField] AudioClip towerBuilt;

    public AudioClip Arrow
    {
        get
        {
            return arrow;
        }
    }

    public AudioClip Rock
    {
        get
        {
            return rock;
        }
    }

    public AudioClip Fireball
    {
        get
        {
            return fireball;
        }
    }

    public AudioClip Death
    {
        get
        {
            return death;
        }
    }

    public AudioClip NewGame
    {
        get
        {
            return newGame;
        }
    }

    public AudioClip Hit
    {
        get
        {
            return hit;
        }
    }

    public AudioClip GameOver
    {
        get
        {
            return gameOver;
        }
    }

    public AudioClip Level
    {
        get
        {
            return level;
        }
    }

    public AudioClip TowerBuild
    {
        get
        {
            return towerBuilt;
        }
    }
}
