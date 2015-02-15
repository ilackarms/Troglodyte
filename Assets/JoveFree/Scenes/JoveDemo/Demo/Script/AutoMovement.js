#pragma strict

var posPoint : Vector3;
var negPoint : Vector3;
var speed : float = 0.01f;
var posMove : boolean = true;

function Start () 
{
	posPoint = this.transform.position;
	negPoint = this.transform.position;
	posPoint.z += 4.0f;
	negPoint.z -= 4.0f;
}

function FixedUpdate () 
{
	if (Vector3.Distance(this.transform.position, posPoint) < 0.1f) posMove = false;
	if (Vector3.Distance(this.transform.position, negPoint) < 0.1f) posMove = true;
	
	if (posMove) this.transform.position.z += speed;
	if (!posMove) this.transform.position.z -= speed;
}