using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject MushRoom;
    [SerializeField] private GameObject DragonNightmare;

    private List<GameObject> Player1Monsters;
    // Start is called before the first frame update
    void Start()
    {
        Player1Monsters = new List<GameObject>();

        //TODO: monster should be spawn next to the summonor;
        Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
        //TODO: monster should be Instantiate after player click the monster icon in magic book
        Player1Monsters.Add((GameObject)Instantiate(MushRoom, spawnPosition, Quaternion.identity));
        Player1Monsters.Add((GameObject)Instantiate(DragonNightmare, spawnPosition, Quaternion.identity));
        Player1Monsters.Add((GameObject)Instantiate(DragonNightmare, spawnPosition, Quaternion.identity));
    }

    // Update is called once per frame
    void Update()
    {
        GameObject monster_select1, monster_select2;
        bool effective_select;
        (effective_select, monster_select1, monster_select2) = DealSelectState();
        if (effective_select)
        {
            Debug.Log(monster_select1.name + " act to " + monster_select2.name);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(Player1Monsters.Count);
            foreach (GameObject monster in Player1Monsters)
            {
                Debug.Log(monster.name);
            }
        }
    }
    private (bool, GameObject, GameObject) DealSelectState()
    {
        GameObject monster_select1, monster_select2;
        bool effective_select;
        Player1SelectManager.Instance.GetSelectState();
        (monster_select1, monster_select2) = Player1SelectManager.Instance.GetSelectState();
        if (monster_select2 != null) {
            Player1SelectManager.Instance.CleanSelect();
            effective_select = true;
            return (effective_select, monster_select1, monster_select2);
        }
        effective_select = false;
        return (effective_select, monster_select1, monster_select2);
    }
}
