using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using System;



public class IK : MonoBehaviour
{
    // Le transform (noeud) racine de l'arbre,
    // le constructeur créera une sphère sur ce point pour en garder une copie visuelle.
    public GameObject rootNode = null;

    // Un transform (noeud) (probablement une feuille) qui devra arriver sur targetNode
    public Transform srcNode = null;

    // Le transform (noeud) cible pour srcNode
    public IKTargetMovement targetMovement;

    // Si vrai, recréer toutes les chaines dans Update
    public bool createChains = true;

    // Toutes les chaines cinématiques
    public List<IKChain> chains = new List<IKChain>();
    // Nombre d'itération de l'algo à chaque appel
    public int nb_ite = 10;



    void kurapika(Transform s, IKJoint same)
    {
        Transform target = null;
        //Transform root = null;
        
        //Peut être rootnode
        //Transform s = articulation.transform;
        Transform CurrentNode = s;







        // Si S est la racine de l'arbre
        if (s.parent == null || chains.Count == 0)
        {

            // Si la racine de l'arbre est une feuille ou le noeud source
            if (s.childCount == 0 || s == srcNode)


            {
                //root = copyroot;
                // Lance une exception avec un message d'erreur
                throw new InvalidOperationException("La racine de l'arbre doit avoir au moins un fils et ne doit pas être égale à srcNode.");
            }

            // Si la racine de l'arbre a au moins un fils
            else
            {
                Debug.Log("BOOOOOOOOOOOUH");
                
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Transform copy = sphere.transform;
                copy.name = "RootTarget";
                copy.GetComponent<Renderer>().material.color = Color.black;
                copy.position = s.position;
                copy.rotation = s.rotation;
                copy.localScale = s.localScale;
                Debug.Log("RootTarget créé à la position: " + copy.position);

                // pour merger les nodes
                //same = new IKJoint(copy);

                while (CurrentNode.childCount == 1)
                {
                    if (CurrentNode != srcNode)
                    {
                        CurrentNode = CurrentNode.GetChild(0);
                    }
                    else
                    {
                        IKChain tmp = new IKChain(s, CurrentNode, copy, targetMovement.target);
                        tmp.Merge(ref same);
                        //chains.Add(tmp);
                        chains.Insert(0, tmp);
                        kurapika(CurrentNode.GetChild(0), tmp.Last());
                        //Debug.Log("CurrentNode " + CurrentNode.name);
                        break;
                    }
                }

                if (CurrentNode.childCount == 0)
                {
                    
                    //Cas classique d'une chaîne unique
                    if (CurrentNode == srcNode)
                    {
                        target = targetMovement.target;
                    }

                    IKChain tmp = new IKChain(s, CurrentNode, copy, target);
                    tmp.Merge(ref same);
                    //chains.Add(tmp);
                    chains.Insert(0, tmp);
                }
                else
                {
                    
                    // Il faut prendre en considération la possbilité où la racine de l'arbre a plusieurs fils donc sa chaine est un singleton                    
                    if (s == CurrentNode)
                    {
                        IKChain tmp = new IKChain(s, s, copy, target);
                        //tmp.Merge(same);
                        //chains.Add(tmp);
                        chains.Insert(0, tmp);
                        for (int i = 0; i < s.childCount; i++)
                    {
                        kurapika(s.GetChild(i), same);
                        

                    }
                    }

                    else 
                    {
                        if (CurrentNode == srcNode)
                        {
                            target = targetMovement.target;
                        }

                    IKChain tmp = new IKChain(s, CurrentNode, copy, target);
                    tmp.Merge(ref same);
                    //chains.Add(tmp);
                    chains.Insert(0, tmp);

                    for (int i = 0; i < CurrentNode.childCount; i++)
                    {
                        kurapika(CurrentNode.GetChild(i), tmp.Last());
                        //Debug.Log("father " + CurrentNode.name);
                        //Debug.Log("child " + CurrentNode.GetChild(i).name);
                    }


                    }
                    
                }

            }

        }

        
        
        // cas d'un noeud qui n'est pas une racine (avec un père)
        else   
        {
            Debug.Log("allo allo");

            // Soit S' appartenant à la chaîne de S.
            // Quand S' est un noeud avec un seul fils
            while (CurrentNode.childCount == 1)
            {
                if (CurrentNode != srcNode)
                {
                    CurrentNode = CurrentNode.GetChild(0);
                }
                else
                {
                    IKChain tmp = new IKChain(s.parent, CurrentNode, null, targetMovement.target);
                    tmp.Merge(ref same);
                    //chains.Add(tmp);
                    chains.Insert(0, tmp);
                    kurapika(CurrentNode.GetChild(0), tmp.Last());
                    break;
                }
            }
            // Quand S' est un noeud avec plusieurs fils
            if (CurrentNode.childCount > 1)
            {
                if (CurrentNode == srcNode)
                    {
                        target = targetMovement.target;
                    }

                IKChain tmp = new IKChain(s.parent, CurrentNode, null, target);
                tmp.Merge(ref same);
                //chains.Add(tmp);
                chains.Insert(0, tmp);

                
                for (int i = 0; i < CurrentNode.childCount; i++)
                {
                    kurapika(CurrentNode.GetChild(i),tmp.Last());
                    //Debug.Log("CurrentNode father " + CurrentNode.name);
                    //Debug.Log("CurrentNode son " + CurrentNode.GetChild(i).name);
                }     
            }

            // S' est une feuille
            else 
            {
                if (CurrentNode == srcNode)
                    {
                        target = targetMovement.target;
                    }

                IKChain tmp = new IKChain(s.parent, CurrentNode, null, target);
                //debug currentnode.name
                //Debug.Log("CurrentNode " + CurrentNode.name);
                tmp.Merge(ref same);
                //chains.Add(tmp);
                chains.Insert(0, tmp);
            }


        }

    }

        




    void Start()
    {

        
        if (createChains)
        {

        kurapika(rootNode.transform, new IKJoint(rootNode.transform));
            
            
            
            
            
        
        //Debug.Log("(Re)Create CHAIN");
        //IKChain(Transform _rootNode, Transform _endNode, Transform _rootTarget, Transform _endTarget)






        createChains = false; // la chaîne est créée une seule fois, au début



            //IKChain(Transform _rootNode, Transform _endNode, Transform _rootTarget, Transform _endTarget)

        



        // TODO :
        // Création des chaînes : une chaîne cinématique est un chemin entre deux nœuds carrefours.
        // Dans la 1ere question, une unique chaine sera suffisante entre srcNode et rootNode.

        // TODO-2 : Pour parcourir tous les transform d'un arbre d'Unity vous pouvez faire une fonction récursive
        // ou utiliser GetComponentInChildren comme ceci :
        // foreach (Transform tr in gameObject.GetComponentsInChildren<Transform>())

        // TODO-2 : Dans le cas où il y a plusieurs chaines, fusionne les IKJoint entre chaque articulation.
        }
    }
    void Update()
    {
        if (createChains)
        Start();
        if (Input.GetKeyDown(KeyCode.I))
        {
            IKOneStep(true);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //Debug.Log("Chains count="+chains.Count);
            foreach (IKChain ch in chains)
            ch.Check();
        }
    }
    void IKOneStep(bool down)
    {
        int j;
        for (j = 0; j < nb_ite; ++j)
        {
            // TODO : IK Backward (remontée), appeler la fonction Backward de IKChain
            // sur toutes les chaines cinématiques.
            // TODO : appliquer les positions des IKJoint aux transform en appelant ToTransform de IKChain
            // IK Forward (descente), appeler la fonction Forward de IKChain
            // sur toutes les chaines cinématiques.

            // TODO : appliquer les positions des IKJoint aux transform en appelant ToTransform de IKChain

            // la transformation se fait à la convergence

            foreach (IKChain ch in chains)
           
            {
                
            
               
                //Debug.Log("avant backward" + ch.First().name + " : " +  ch.First().position + " // "+ ch.Last().name + " : " + ch.Last().position);


                ch.Backward();
                
                

                //Debug.Log("après backward" + ch.First().name + " : " +  ch.First().position + " // "+ ch.Last().name + " : " + ch.Last().position);



                
                ch.ToTransform(); 

                //Debug.Log("après transformation" + ch.First().name + " : " +  ch.First().position + "//"+ch.Last().name + " : " + ch.Last().position);





                //ch.test2();
                 // Add this line

                ch.Forward();

                //ch.ToTransform(); 
                                //Debug.Log("après forward" + ch.First().name + " : " +  ch.First().position + " // "+ ch.Last().name + " : " + ch.Last().position);

                                //écrire la distance entre srcnode et tartgetnode
                float distance = Vector3.Distance(srcNode.position, targetMovement.target.position);
                Debug.Log("Distance entre srcnode et tartgetnode : " + Vector3.Distance(srcNode.position, targetMovement.target.position));
                //ch.ToTransform();
                Debug.Log("après forward; last transform" + ch.First().name + " : " +  ch.First().position + " // "+ ch.Last().name + " : " + ch.Last().position);
                Debug.Log("après forward; last positions transform" + ch.First().name + " : " +  ch.First().positionTransform + " // "+ ch.Last().name + " : " + ch.Last().positionTransform);


                //ch.test();
            }


             foreach (IKChain ch in chains)
            {
                //ch.cylinderisation();
            } 


             


        }
    }
}
