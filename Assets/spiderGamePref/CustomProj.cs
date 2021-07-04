using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomProj : MonoBehaviour
{
    public GameObject Explosion;
    public float Timer = 1f;
    public float TimerEXP = 0.2f;

    public bool Detach = false;
    void Start()
    {
        Destroy(gameObject, Timer);
    }

    void Exp()
    {
        if (Detach)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject, 1f);
                child.parent = null;
            }
        }
        Destroy(gameObject);
        GameObject E = Instantiate(Explosion, transform.position, Explosion.transform.rotation);
        Destroy(E, TimerEXP);
    }
    void Update()
    {
        if(gameObject.transform.position.z>4.0f){
            Destroy(gameObject);
        }
            Debug.Log("print data out" + gameObject.transform.position.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        //Debug.Log(collision.gameObject.tag);
        //if (collision.gameObject.tag == "spider"  && collision.gameObject.tag != "fireball") 
        //{
       
          if(collision.gameObject.tag != "fireball"){
              //   Exp();
            Debug.Log(collision.gameObject.tag);
          }
          if (collision.gameObject.tag == "spiderObj" ){
              Animator anim =  collision.gameObject.GetComponent<Animator>();
              anim.Play("die");
                     Destroy(gameObject);
          }
        //}
    }
    
 
}
