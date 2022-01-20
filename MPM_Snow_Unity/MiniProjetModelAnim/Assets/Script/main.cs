//François de Weyer


using System;
using ParticuleSnow;
using CellSnow;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour
{
	
	public Particule[] particules;
	public Cell[,,] grid;
	public Rigidbody[] lParticule;
	public Matrix<double>[] fEl, fPl;
	// dimension grille
	public int ecranWidth = Screen.width;
	public int ecranHeight = Screen.height;
	public int ecranDepth = 400;

	// parametre
	public float alpha = 0.95f;
	public float ccompr = 0.025f;
	public float cstrect = 0.0075f;
	public float coeff_hard = 10f;
	public float dens_ini = 400f;
	public float young_mod = 140000f;
	public float poisson_ratio = 0.2f;

	// grille
	public int sizeCell = 16;

	// temps
	public bool firstTimeStep=true;
	public float dt = 0; // temps actuelle


	void Start()
	{
		grid = new Cell[ecranWidth, ecranHeight, ecranDepth];
		InitGrille();

		lParticule = GameObject.Find("spawner1").GetComponent<createParticule>().getList();
		fEl = GameObject.Find("spawner1").GetComponent<createParticule>().getFel();
		fPl = GameObject.Find("spawner1").GetComponent<createParticule>().getFpl();
		InitParticule();
		InitGrille();
	}

	void Update()
	{
		
	}


	void fixedUpdate()
	{
		if(dt < 0.1) { 
		// Preparation 
		InitGrille();

		// Etape 1 : Rasterisation de la vitesse et du poids
		for(int i=0;i<particules.Length;i++)
		{
			// On determine la position de la particule de la grille
			// Correspond à l'index (i,j,k) dans le tableau
			Vector3 index = getIndex(particules[i].pos,sizeCell);

			// Determination des interpolations

			particules[i].setWeight(bSpline(particules[i].pos,index,sizeCell));
			grid[(int)index.x,(int)index.y,(int)index.z].setWeight(bSplineDeriv(particules[i].pos,index,sizeCell));

			// Calcule vitesse et masse dans la grille
			grid[(int)index.x,(int)index.y,(int)index.z].Init(particules[i].vit, particules[i].mas, particules[i].weight);
			
			// Etape 2 : determination du volume et de la densité (A la première itération seulement)
			if(this.firstTimeStep)
			{
				particules[i].InitVolDens(grid[(int)index.x,(int)index.y,(int)index.z].mas,sizeCell);
			}
		}
		this.firstTimeStep = false;

		for(int i=0;i<particules.Length;i++)
		{
			Vector3 index = getIndex(particules[i].pos,sizeCell);

			// Etape 3 : Calcul des forces
			float mu = young_mod / (2 * (1 + poisson_ratio));
			float lambda = young_mod * poisson_ratio / ((1 + poisson_ratio) * (1 - 2 * poisson_ratio));
			grid[(int)index.x, (int)index.y, (int)index.z].updateForce(fEl[i],fPl[i], particules[i].vol, mu,lambda ,coeff_hard);
			lParticule[i].AddForce(grid[(int)index.x, (int)index.y, (int)index.z].f);

			// Etape 4 : Mise a jour de la vitesse dans la grille
			grid[(int)index.x,(int)index.y,(int)index.z].updateVit(dt);

			// Etape 5 : Gestion des collisions dans la grille


			// Etape 6 : Update de la vitesse implicite 
			// Ici, on choisit une vitesse explicite donc pas d'update

		}
		
		for(int i=0;i<particules.Length;i++)
		{
			Vector3 index = getIndex(particules[i].pos,sizeCell);

			// Etape 7 : Mise a jour de la deformation de gradient
			Matrix<double> newFela = DenseMatrix.OfArray(new[,] {
				{1.0,0.0,0.0},
				{0.0,1.0,0.0},
				{0.0,0.0,1.0}
				});
			newFela += dt * fEl[i];

			Matrix<double> newFpl = newFela * fEl[i];


			var sigma = newFela.Svd().S;
			float omegaC = 1 - ccompr;
			float omegaS = 1 + cstrect;

			sigma[0] = (float) Mathf.Clamp((float)sigma[0], omegaC, omegaS);
			sigma[1] = (float) Mathf.Clamp((float)sigma[1], omegaC, omegaS);
			sigma[2] = (float) Mathf.Clamp((float)sigma[2], omegaC, omegaS);

			Matrix<double> sigmaMatrix = DenseMatrix.OfArray(new[,] {
				{sigma[0],0.0,0.0},
				{0.0,sigma[1],0.0},
				{0.0,0.0,sigma[2]}
				});

			particules[i].updateFela((Matrix<double>) newFela.Svd().U * sigmaMatrix * newFela.Svd().VT);
			particules[i].updateFpl(newFela.Svd().VT * sigmaMatrix.Inverse() * newFela.Svd().U.Transpose() * newFpl);

			// Etape 8 : Mise a jour de la vitesse de la particule			
			Vector3 pic = vitPIC(grid[(int)index.x,(int)index.y,(int)index.z].vit,particules[i].weight);
			
			Vector3 flip = vitFLIP(grid[(int)index.x,(int)index.y,(int)index.z].vit,grid[(int)index.x,(int)index.y,(int)index.z].oldVit
				,particules[i].vit,particules[i].weight);

			particules[i].updateVit(alpha,pic,flip);
			// Etape 9 : Gestion des collisions entre les particules


			// Etape 10 : Mise a jour de la position
 			particules[i].updatePos(dt);


		}	
 		// reset le tableau grid et du poids des particules
		Array.Clear(grid, 0, grid.Length);
		for(int i=0;i<particules.Length;i++)
		{
			particules[i].resetWeight();

			// Changement des valeurs dans les objets
			lParticule[i].position = particules[i].pos;
			lParticule[i].velocity = particules[i].vit;
			lParticule[i].mass = particules[i].mas;
			fEl[i] = particules[i].fEl;
			fPl[i] = particules[i].fPl;

			GameObject.Find("spawner1").GetComponent<createParticule>().setList(lParticule);
			GameObject.Find("spawner1").GetComponent<createParticule>().setFpl(fPl);
			GameObject.Find("spawner1").GetComponent<createParticule>().setFel(fEl);

		}
			dt += Time.fixedDeltaTime;
		}
	}


	// Fonction de traitement

	// Permet de calculer le poids de la position d'une particule dans la grille 
		public float bSpline(Vector3 pos, Vector3 index, float cellsize)
	    {
	        float Ni = (1 / cellsize) * (pos.x - index.x * cellsize);
	        float Ny = (1 / cellsize) * (pos.y - index.y * cellsize); 
	        float Nz = (1 / cellsize) * (pos.z - index.z * cellsize);

	        return N(Ni) * N(Ny) * N(Nz);
	    }

	    public float bSplineDeriv(Vector3 pos, Vector3 index, float cellsize)
	    {
	        float Ni = (1 / cellsize) * (pos.x - index.x * cellsize);
	        float Ny = (1 / cellsize) * (pos.y - index.y * cellsize); 
	        float Nz = (1 / cellsize) * (pos.z - index.z * cellsize);

	        return derivN(Ni) * derivN(Ny) * derivN(Nz);
	    }

	    // Fonction N 
	    public float N(float x)
	    {
	        float abx = System.Math.Abs(x);
	        if (abx < 0)
	        {
	            return 0;
	        }
	        if (abx < 1)
	        {
	            return (1 / 2) * (abx * abx * abx) - (x * x) + (2 / 3);
	        }
	        return -(1/6) * (abx * abx * abx) +(x*x) - 2 * abx + 4/3;
	    }


	    // Dérivée de la fonction N
	    public float derivN(float x)
	    {
	    	float abx = System.Math.Abs(x);
	        if (abx < 0)
	        {
	            return 0;
	        }
	        if (abx < 1)
	        {
	            return (x * abx*abx *3)/2 - 2*x;
	        }
	        return -(x*abx*abx)/2 + 2*x - 2;
	    }



	// Permet de calculer la nouvelle vitesse d'une particule (a partir de la vitesse calculée dans la grille)
	    public Vector3 vitPIC(Vector3 v, float posWeight)
	    {
	    	return v*posWeight;

	    }
	    public Vector3 vitFLIP(Vector3 vGrid, Vector3 oldvGrid, Vector3 v, float posWeight)
	    {
	    	return v + (vGrid-oldvGrid)*posWeight;	
	    }


	// Calcul des coordonnées des particules dans la grille ( calculer dans Init )
	   public Vector3 getIndex(Vector3 pos,float sizeCell)
	   {
	   		Vector3 index = new Vector3(0,0,0);
	   		index.x = (int) System.Math.Floor(pos.x/sizeCell);
	   		index.y = (int) System.Math.Floor(pos.y/sizeCell);
	   		index.z = (int) System.Math.Floor(pos.z/sizeCell);
	   		return index;
	   }


		public void InitParticule()
		{
			for (int i = 0; i < lParticule.Length; i++)
			{
				Particule p = new Particule(lParticule[i].position, lParticule[i].mass, lParticule[i].velocity
					,fEl[i],
					fPl[i]);
				particules[i] = p;
			}

		}
		public void InitGrille()
		{
			for (int i = 0; i < Screen.width; i++)
			{
				for (int j = 0; j < Screen.height; j++)
				{
					for (int k = 0; k < ecranDepth; k++)
					{
						Cell c = new Cell();
						grid[i, j, k] = c;
					}
				}
			}
		}




}