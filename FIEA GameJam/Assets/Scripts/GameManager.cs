using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //gems
    public int gemsMinedRed;
    public int gemsMinedGreen;
    public int gemsMinedBlue;

    //upgrades 
    public int shotgunLevel = 0;
    public int pickaxeLevel = 0;
    public int playerHealthLevel = 0;
    public int playerSpeedlevel = 0;
    public int playerDefencelevel = 0;
    public int torchLimitLevel = 0;

    //money
    public int money = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
