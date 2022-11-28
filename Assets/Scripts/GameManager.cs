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
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(Player1Monsters.Count);
            foreach (GameObject monster in Player1Monsters)
            {
                Debug.Log(monster.name);
            }
        }
    }
}
