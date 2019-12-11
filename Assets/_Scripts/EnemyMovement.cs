using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] ParticleSystem EndParticle;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float lookSpeed = 5.0f;
    [SerializeField] List<Waypoint> path;

    private Transform toGoTo;

    // Use this for initialization
    void Start()
    {
        //pathFinder is my waypoint calculator / breadths first search calculator
        Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
        //path is the list of waypoints in pathfinder from start to finish and is retrieved with a function to avoid unwanted editing 
        path = pathfinder.GetPath();
        //starts the followpath routine and passes it the path list from pathfinder
        StartCoroutine(FollowPath(path));
    }

    void Update()
    {
        Move();
    }

    //runs every frame for smooth movement (could put in Update but if I needed to expand script/ add more functions to the update I would have to do this anyways)
    void Move()
    {
        float MovementSpeed = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, toGoTo.position, MovementSpeed);

        Vector3 relativePos = toGoTo.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, lookSpeed * Time.deltaTime);
    }

    IEnumerator FollowPath(List<Waypoint> wppath)
    {
        foreach (Waypoint waypoint in wppath)
        {
            toGoTo = waypoint.transform;
            yield return new WaitUntil(() => isAtPosition(waypoint) == true);
        }
        ReachedEnd();
    }

    //returns true when enemy has reached the position of the next waypoint
    bool isAtPosition(Waypoint wp)
    {
        if (this.transform.position == wp.transform.position)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //things to do once object has reached end tile
    void ReachedEnd()
    {
        var deathParticle = Instantiate(EndParticle, this.transform.position, Quaternion.identity, this.transform.parent.transform);
        deathParticle.Play();
        Destroy(deathParticle.gameObject, deathParticle.main.duration);
        Destroy(this.gameObject);
    }

    //just a setter for scriptable objects
    public void setSpeed(float speed)
    {
        moveSpeed = speed;
    }

}