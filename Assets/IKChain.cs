using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class IKChain
{
    

    // Quand la chaine comporte une cible pour la racine.
    // Ce sera le cas que pour la chaine comportant le root de l'arbre.
    private IKJoint rootTarget = null;

    // Quand la chaine à une cible à atteindre,
    // ce ne sera pas forcément le cas pour toutes les chaines.
    private IKJoint endTarget = null;
    // Toutes articulations (IKJoint) triées de la racine vers la feuille. N articulations.
    private List<IKJoint> joints = new List<IKJoint>(); 

    // Contraintes pour chaque articulation : la longueur (à "modifier" pour
    // ajouter des contraintes sur les angles). N-1 contraintes.
    private List<float> constraints = new List<float>();


    // Un cylindre entre chaque articulation (Joint). N-1 cylindres.
    private List<GameObject> cylinders = new List<GameObject>();

    // Créer la chaine d'IK en partant du noeud endNode et en remontant jusqu'au noeud plus haut, ou jusqu'à la racine
    public IKChain(Transform _rootNode, Transform _endNode, Transform _rootTarget, Transform _endTarget)
    {

        Debug.Log("=== IKChain::createChain: ===");
        // TODO : construire la chaine allant de _endNode vers _rootTarget en remontant dans l'arbre (ou l'inverse en descente).
        // Chaque Transform dans Unity a accès à son parent 'tr.parent'

        // L'ordre de chaque chaque est du parent vers l'enfant, jusqu'à une feuille ou un noeuds avec plusieurs enfants.
        // Il suffit d'utiliser la fonction name pour vérifier si c'est null (en string)
        Transform currentNode = _endNode;
        while (currentNode.name != _rootNode.name && currentNode.parent != null)
        {
                joints.Insert(0,new IKJoint(currentNode));
                Vector3 vect = currentNode.position - currentNode.parent.position;
                
                constraints.Insert(0,vect.magnitude);

                

                // créer un cylindre entre currentNode et currentNode.parent pour visualiser la contrainte (taille distance etc...)
                //GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                
               
               
                //cylinder.transform.position = (currentNode.transform.position + currentNode.parent.transform.position)/ 2;
                //cylinder.transform.localScale = new Vector3(0.1f, constraints[0] / 2, 0.1f);
            

            
                //cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, currentNode.parent.position - currentNode.position);
            
            

            //cylinders.Insert(0,cylinder);

           

            
           
            currentNode = currentNode.parent;
            //Debug.Log("check it" + currentNode.name); 

            
        }

        // Si la racine est atteinte, il faut créer un joint pour la racine

        joints.Insert(0,new IKJoint(_rootNode));
        
        
        
        
        
        
        if (_endTarget != null)
        {
            endTarget = new IKJoint(_endTarget);
        }
        else {
            endTarget = new IKJoint();
        }
        
        
        if (_rootTarget != null)
       

        {
            rootTarget = new IKJoint(_rootTarget);
        }


        else 
        {
            rootTarget = new IKJoint();
        }






    //_rootTarget.rotation = Quaternion.Euler(_rootNode.rotation.eulerAngles);
    //_rootTarget.localScale = new UnityEngine.Vector3(_rootNode.localScale.x, _rootNode.localScale.y, _rootNode.localScale.z);

    //UnityEngine.Vector3 targetpos = new UnityEngine.Vector3(_rootNode.position.x, _rootNode.position.y, _rootNode.position.z);

    }


    

    public void Merge(ref IKJoint j)
    
    //{joints[0] = j;}
    
    
    {
    if (First().transform.parent != null && rootTarget.name != "null") 
    {
        joints[0] = j;
        
        
        

    }
    else
    {
        rootTarget = j;
    }
    } 

    

    
    public IKJoint First()
    {
        return joints[0];
    }
    public IKJoint Last()
    {
        return joints[ joints.Count-1 ];
    }
    public void Backward()
    {
        // TODO : une passe remontée de FABRIK. Placer le noeud N-1 sur la cible,
        // puis on remonte du noeud N-2 au noeud 0 de la liste
        // en résolvant les contrainte avec la fonction Solve de IKJoint.

        //public void Solve(IKJoint anchor, float l)
        
        //Si la chaine de la racine est un singleton il ne se passera rien puisque puisque roottarget est en merge avec ses firsts de ses fils       
        
        // effectuer le changement aussi sur les cylindres        

        
        
        if (joints.Count != 1)
        {              
            
         
         // Si cette chaîne contient une feuille sans cible

            if ((endTarget.name == "null") && (Last().transform.childCount == 0))
            {
                    // On incrémente juste le poids du père de la chaîne qui est un père commun
                   //Vector3 v = Vector3.zero;
                   First().AddPosition(First().position);

            }
         
         
           

            else {

                
                 int o = 0;
                
                //Si le noeud source est la feuille de la chaine  
                if (endTarget.name != "null")
                {
                    
                    endTarget.SetPosition(endTarget.transform.position);
                    
                    
                    Last().SetPosition(endTarget.position);
                    o += 1;
                }

                // Au cas où le dernier joint contient plusieurs fils on pondére la position de la feuille par le nombre de fils
                //Last().ToTransform();
                Debug.Log("before backward" + Last().name+ "pos"+Last().position+ "  // " + First().name+"pos" + First().position);
                
                // Backward de la dernière jusqu'au 2ème noeud
                Debug.Log(Last().name+ " last to first "+ First().name);

                //Last().ToTransform();



                // l'incrémentation se fait de l'avant dernier noeud au premier fils
                for (int i = joints.Count - 2; i >= o; i--)
                {
                   
                    //joints[i].Solve(joints[i+1], constraints[i],cylinders[i]);
                    joints[i].Solve(joints[i+1], constraints[i]);
                    
                }

                //Cas de la racine 
                if (rootTarget.name != "null")
                {
                    Debug.Log("roottarget before backward" + rootTarget.position);
                    //rootTarget.Solve(joints[1], constraints[0],cylinders[0]);
                    rootTarget.Solve(joints[1], constraints[0]);
                    Debug.Log("roottarget after backward" + rootTarget.position);
                    //Debug.Log("origin after backward" + First().position);
                    
                    //Copie de la racine au backward
                    rootTarget.ToTransform();


                    First().SetPosition(First().transform.position);
                }
                
                //Parent de chaîne classique
                else {
                    //First().Solve(joints[1], constraints[0],cylinders[0]);
                    First().Solve(joints[1], constraints[0]);
                    }
                }

                Debug.Log("after  backward" + Last().name+ "pos"+Last().position+ "  // " + First().name+"pos" + First().position+ "rootar" + rootTarget.position);
            }

        // dans le cas où la racine de l'arbre a plusieurs fils directs, c'est le seul cas où il peut y avoir une chaine avec un seul joint
        
        else
        {           
            
            First().SetPosition(First().transform.position);
            //Copie au backward
            rootTarget.ToTransform();

        }

        
        
        //First().ToTransform();

    }




    public void Forward()

    {

        // TODO : une passe descendante de FABRIK. Placer le noeud 0 sur son origine puis on descend.
        // Codez et deboguez déjà Backward avant d'écrire celle-ci.

        //positionTransform
        //First().SetPosition(First().positionTransform);
       
        

   

        
        if (joints.Count != 1)
        {
            float dist =0.0f;

           
            
                        
            for (int i = 1; i < joints.Count; i++)
            {
                //joints[i-1].ToTransform();
                //joints[i].Solve(joints[i-1], constraints[i-1],cylinders[i-1]);                
                joints[i].Solve(joints[i-1], constraints[i-1]);


                //joints[i].ToTransform(); 
                dist = Vector3.Distance(joints[i].position,joints[i-1].positionTransform);
                Debug.Log("distance between " + joints[i].name + "and " + joints[i].position + ": " + dist + "vs " + constraints[i-1]); 


            // appliquer des contraintes de rotation entre les joints, rotation toward, quaternion etc
            //Vector3 directionToTarget = joints[i].position - joints[i-1].position;
            //Quaternion rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.up,directionToTarget);
           //joints[i-1].transform.rotation = rotation;
            

            




                            
            }
           /* if (endTarget.name != "null")
        {
            Vector3 directionToTarget = endTarget.position - joints.Last().position;
            //Quaternion rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            Quaternion rotationn = Quaternion.LookRotation(directionToTarget);
           joints.Last().transform.rotation = rotationn;


            
        }  */


         if (endTarget.name != "null")
        {
            Debug.Log("HOLAAAAAAAAAAAAAAAAAAAAAAAA " + joints.First().name + "and " + joints.Last().name );
            Vector3 directionToTarget = endTarget.position - joints.Last().positionTransform;
            //Quaternion rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            //Quaternion rotationn = Quaternion.LookRotation(directionToTarget);


            Quaternion rotationn = Quaternion.LookRotation(directionToTarget);
            //rotationn *= Quaternion.Euler(90, 0, 0);

           //joints.Last().transform.rotation = rotationn;
           //endTarget.transform.rotation = rotationn;


           


            
        }  
            joints.Last().ToTransform();
                 
            


        







            Debug.Log("POSITIONNN " + joints.Last().name + "and " + joints.Last().position); 

            //cylinders[cylinders.Count-1].transform.position = (joints.Last().positionTransform + joints[joints.Count-2].position)/2;
            

            
            
              
        }

        Debug.Log("correct values  =====");
        
    

    }



    public void ToTransform()
    {
        
        foreach (IKJoint joint in joints)
        
          {  
            joint.ToTransform();

          }
        

        //cylindre.transform.position = (anchor.position + position)/2;
        //cylindre.transform.up = direction;
        //cylindre.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);


        // TODO : pour tous les noeuds de la liste appliquer la position au transform : voir ToTransform de IKJoint

        //First().ToTransform();
         //for (int i=1;i<joints.Count(); i++)
        
        //Vector3 direction = (joints[i].position - joints[i-1].position).normalized;
        //Vector3 newPosition = anchor.position - direction * l;    

        
        //joints[i].ToTransform();
        //cylinders[i-1].transform.position = (joints[i-1].positionOrigParent + joints[i-1].position)/ 2;  }
                
            //joints[i+1].transform;
            //IKJoint j=joints[i+1].position;

            
            //joints[i+1].positionOrigParent;

                
          //  Debug.Log("Chaine " +First().name + "   " + Last().name);  // écrire
            //float distance = Vector3.Distance(joints[i].transform.position,joints[i+1].transform.position);
            //Vector3 direction = (joints[i].transform.position-joints[i+1].transform.position).normalized;
            //Debug.Log(v2.name + "and "+ joints[i].name + "  real   " + distance+" and wanted  " + constraints[i]);
            
            //cylinders[i].transform.SetParent(joints[i].transform);

            //cylinders[i].transform.position = (joints[i].transform.position+joints[i+1].transform.position)/2;
            //cylinders[i].transform.up = direction;
            //cylinders[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            //cylinders[i].transform.SetParent(joints[i].transform);
            //cylinders[i].transform.localScale = new Vector3(0.1f, constraints[i] / 2, 0.1f);
        
        
        
     
    }


    
    public void cylinderisation()
    {
        // relie les spheres entre elles par des cylindres déjà crés pour visualiser la chaine


        
        
   /* 
     
        
        for (int i = 0; i < joints.Count - 1; i++)
        {
            /* cylinders[i].transform.position = (joints[i].positionTransform + joints[i+1].positionTransform) / 2;
            cylinders[i].transform.up = joints[i+1].positionTransform - joints[i].positionTransform;
            cylinders[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, joints[i+1].positionTransform - joints[i].positionTransform);
            



            Vector3 directionToTarget = (joints[i+1].positionTransform - joints[i].positionTransform).normalized;
            
            //Vector3 directionToTargetti = (endTarget.position - joints[i].positionTransform).normalized;

            //Quaternion rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.up,directionToTarget);
            Quaternion rotation = Quaternion.LookRotation(-directionToTarget);
           joints[i].transform.rotation = rotation;
        }



                if (endTarget.name != "null")
        {
            Vector3 relativePos = endTarget.positionTransform - joints.Last().position;
            //Quaternion rotationn = Quaternion.FromToRotation(Vector3.forward,relativePos);
            Quaternion rotationn = Quaternion.LookRotation(relativePos, Vector3.up);

            joints.Last().transform.rotation = rotationn;
        }

        */
       
    }
    

    public void Check()
    {
        // TODO : des Debug.Log pour afficher le contenu de la chaine (ne sert que pour le debug)
        // afficher le nombre de joints et le nom de chacune des articulations sous la forme (inversé): joinN.name <= joinN-1.name <= joinN-1.name <= ...
        Debug.Log("Number of joints: " + joints.Count);
        string chainContent = "";
        for (int i = joints.Count - 1; i >= 0; i--)
        {
            chainContent += joints[i].name;
            if (i > 0)
            {
                chainContent += " <= ";
            }
        }
        Debug.Log("Chain content: " + chainContent);
    
    }



}






    









