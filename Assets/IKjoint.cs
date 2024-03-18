using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public class IKJoint
{
    // la position modifiée par l'algo : en fait la somme des positions des sous-branches.
    // _weight comptera le nombre de sous-branches ayant touchées cette articulation.
    private Vector3 _position;

    // un lien vers le Transform de l'arbre d'Unity
    private Transform _transform;

    // un poids qui indique combien de fois ce point a été bougé par l'algo.
    private float _weight = 0.0f;
    
    public string name
    {
        get         
        {
        return _transform != null ? _transform.name : "null";
        }         
    }
    public Vector3 position // la position moyenne, il faut prendre en considération la position initiale 
    //penser à une pondération, barycentre
    {
       // TODO : retourne la position moyenne, en divisant par _weight
        get
        {
           //Debug.Log("poids de " + name + " // " + _weight + " //");
            if (_weight == 0) 
           
                return _position;

            else

                    return _position/_weight;

                    //return (_position - positionTransform) / _weight;
        }
                    
    }
    
    public Vector3 positionTransform
    {
        get
        {
            return _transform.position;
        }
    }
    public Transform transform
    {
        get
        {
            return _transform;
        }
    }
    public Vector3 positionOrigParent
    {
        get
        {
            return _transform.parent.position;
        }
    }
    public IKJoint(Transform t)
    {
        // TODO : initialise _position, _weight
        _transform = t;
        _position = t.position;
        _weight = 0.0f;
    }
    // et pour le cas null ?
    public IKJoint()
    {
        _transform = null;
        _position = Vector3.zero;
        _weight = 0.0f;
    }


    public void SetPosition(Vector3 p)
    {
        // TODO
        _position = p;
        _weight = 0.0f;
    }


// des variantes de setposition que j'ai décidé de créer pour simplifier
     public void SetPosition(Transform t1)
    {
        // TODO
        _position = t1.position;
        _weight = 0.0f;
    }


         public void SetPosition(IKJoint j)
    {
        Vector3 tmp = j.position;
        _position = tmp;
        _weight = 0.0f;
    }

        public void SetPosition()
        {
            // TODO
            _position = positionTransform;
            _weight = 0.0f;
        }



    public void AddPosition(Vector3 p)
    {
        // TODO : ajoute une position à 'position' et incrémente '_weight'

        switch (_weight)
        {
            case 0:
                _position = p;
                _weight = 1.0f;
                break;
            default:
                _position += p;
                _weight += 1.0f;
                break;
        }
        
    }
    
    
    public void ToTransform()
    {
        // TODO : applique la _position moyenne au transform, et remet le poids à 0
        _transform.position = position;
        _weight = 0.0f;
    }
    
    //public void Solve(IKJoint anchor, float l, GameObject cylindre)
    public void Solve(IKJoint anchor, float l)
    {
        // TODO : ajoute une position (avec AddPosition) qui repositionne _position à la distance l
        // en restant sur l'axe entre la position et la position de anchor
        //  régule le cylindre entre les deux joints aussi
        
        // Seulement si le joint n'est pas une feuille
       
        //if (anchor.transform.childCount > 0)
            
        anchor.ToTransform();

        Debug.Log(name + " of Anchor SOLVE " + anchor.name + " " + anchor.position);    
    
        Debug.Log("POSITION AVANT SOLVE " + name + " " + position);    
                       
        Vector3 direction = (anchor.position - positionTransform).normalized;
        Vector3 newPosition = anchor.position - direction * l;      


        Debug.Log("POSITION INTERNE SOLVE " + name + " " + newPosition);    



        AddPosition(newPosition);
        //Debug.Log(name + " " + positionTransform);     
        Debug.Log("POSITION APRES SOLVE " + name + " " + position);                              





        Vector3 directionToTarget = position - anchor.position ;
        //Quaternion rotation = Quaternion.FromToRotation(directionToTarget, -Vector3.up);
        //Quaternion rotation = Quaternion.FromToRotation(-Vector3.up,-direction);
        Quaternion rotation = Quaternion.LookRotation(direction);
        //Quaternion rotation = Quaternion.LookRotation(position);

        //rotation *= Quaternion.Euler(0, 90, 0);
        //anchor.transform.rotation = rotation;


    }


}




    





