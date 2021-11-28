using UnityEngine;

public class Hitpoint : MonoBehaviour
{
    bool hasController;
    GrayCharacterController charController;

    private int _curHP;
    private int _maxHP;

    public int curHP
    {
        get => _curHP;
    }
    public int maxHP
    {
        get => _maxHP;
        set => _maxHP = value;
    }

    public void damage(int dmg)
    {
        if (charController.isRolling()) return;
        _curHP = System.Math.Max(_curHP - dmg, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<GrayCharacterController>();
        hasController = (charController != null);
    }
}
