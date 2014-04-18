using UnityEngine;
using System.Collections;

public class SoundfxScript : MonoBehaviour {

    public GameObject deathSound;
    public GameObject swooshSound;
    public GameObject Melee_HitSound;
    public GameObject Rivet_GunSound;
    public GameObject PistolSound;

    public enum soundType { DEATH, SWOOSH, MELEE_HIT, RIVET_GUN, PISTOL };
    public void instantiateSound(Vector3 loc, soundType type)
    {
        GameObject GO;
        switch (type)
        {
            case soundType.DEATH:
                GO = (GameObject)GameObject.Instantiate(deathSound);
                GO.transform.position = loc;
                break;
            case soundType.SWOOSH:
                GO = (GameObject)GameObject.Instantiate(swooshSound);
                GO.transform.position = loc;
                break;
            case soundType.MELEE_HIT:
                GO = (GameObject)GameObject.Instantiate(Melee_HitSound);
                GO.transform.position = loc;
                break;
            case soundType.RIVET_GUN:
                GO = (GameObject)GameObject.Instantiate(Rivet_GunSound);
                GO.transform.position = loc;
                break;
            case soundType.PISTOL:
                GO = (GameObject)GameObject.Instantiate(PistolSound);
                GO.transform.position = loc;
                break;
        }
    }
}
