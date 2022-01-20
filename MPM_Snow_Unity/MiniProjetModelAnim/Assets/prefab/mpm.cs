using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParticuleSnow;
using CellSnow;


public class mpm : MonoBehaviour
{
    public Cell [,,] grille;
    public Particule[] particules;
    public Rigidbody[] lParticule;

    // dimension grille
    public int ecranWidth = Screen.width;
    public int ecranHeight = Screen.height;
    public int ecranDepth = 400;

    public float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        grille = new Cell[ecranWidth,ecranHeight,ecranDepth];
        InitGrille();

        lParticule = GameObject.Find("spawner1").GetComponent<createParticule>().getList();
        InitParticule();
        InitGrille();
    }

   
    void InitParticule()
    {
        for (int i = 0; i < lParticule.Length; i++)
        {
            Particule p = new Particule(lParticule[i].position, lParticule[i].mass, lParticule[i].velocity
                    , GameObject.Find("spawner1").GetComponent<createParticule>().getFel()[i],
                    GameObject.Find("spawner1").GetComponent<createParticule>().getFpl()[i]);
            particules[i] = p;
        }

    }
    void InitGrille()
    {
        for (int i = 0; i < Screen.width; i++)
        {
            for(int j=0; j<Screen.height;j++)
            {
                for(int k = 0; k < ecranDepth; k++) { 
                    Cell c = new Cell();
                    grille[i,j,k] = c;
                }
            }
        }
    }


















    // Update is called once per frame
    void Update()
    {

    }


    void FixedUpdate() // a utiliser pour ajouter des forces
    {


    }
}
