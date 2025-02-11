using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Sorcerer : MonoBehaviour
{
    public float FollowDist;
    public GameObject EnemyDeath;
    // public GameObject Enemy;
    public GameObject Projectile;
    //public 
    private Transform Player;
    public Transform Head;
    public Transform ShootLoc;
    public float ShootCoolDown;
    NavMeshAgent Agent;
    private float time = 0.0f;
    public HealthBar HealthBar;
    private float Health = 100.0f;
    public float MaxHealth = 200.0f;
    public float FireDistance = 100.0f;

    public GameObject model;
    public Animator anim;

    private GamePlayManager manager;
    private bool EnemyEngaged = false;
    public void DecreaseHealth(float damage)
    {

        Health -= damage;
        HealthBar.SetHealthBar(Health/MaxHealth);
        if (Health < 0.0f)
        {
            //Instantiate(EnemyDeath, this.transform.position, this.transform.rotation);
            
            anim.enabled=false;
            model.transform.parent = null;
            manager.EnemyKilled();
            Destroy(transform.parent.gameObject);
            //Destroy(Enemy);
        }
        else
        {
            HealthBar.SetHealthBar(Health/MaxHealth);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Transform>();
        manager = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GamePlayManager>();
    }

    bool Attacking = false;
    IEnumerator FireBallCooldown() 
    {
        Attacking = true;
        anim.SetBool("Attack", true);
        yield return new WaitForSeconds(0.45f); 
        
        var rot = this.transform.rotation * Quaternion.Euler(0, 180f, 0);
        GameObject o = Instantiate(Projectile, ShootLoc.position, rot);
        o.GetComponent<Rigidbody>().velocity = 30.0f * (ShootLoc.position - Head.position);
        
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Attack", false);
        yield return new WaitForSeconds(ShootCoolDown);

        Attacking = false;

    }

    bool Stopped = false;
    IEnumerator StopNav() 
    {
        anim.SetInteger("MoveState", 0);
        Stopped = true;
        Agent.Stop();
        yield return new WaitForSeconds(1);
        Agent.Resume();
        Stopped = false; 
        //anim.SetInteger("MoveState", 1);
    }
    // Update is called once per frame
    void Update()
    {
        
        float Dist2 = (this.transform.position - Player.position).sqrMagnitude;
        if ( Dist2< FollowDist)
        {
            //anim.SetInt("MoveState",1);
            if (!EnemyEngaged) { EnemyEngaged = true; manager.EnemyEngaged(); }


            if (Dist2 > FireDistance)
            {
                Agent.SetDestination(Player.position);
                anim.SetInteger("MoveState", 1);
            }
            else {
                if (Stopped) { this.transform.LookAt(new Vector3(Player.position.x, this.transform.position.y, Player.position.z), Vector3.up); } else 
                { StartCoroutine(StopNav()); }
                 }
            if (!Attacking) 
            {
                StartCoroutine(FireBallCooldown());
            }
           
        }
        else { }

    }

}

