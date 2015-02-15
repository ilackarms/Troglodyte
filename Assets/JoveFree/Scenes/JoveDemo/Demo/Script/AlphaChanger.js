#pragma strict

var mat : Material;
var alpha : float;
var incr : float = 0.015f;
var goUp : boolean = true;

function Start () 
{
	alpha = this.gameObject.renderer.sharedMaterial.color.a;
}

function FixedUpdate()
{
	if (alpha >= 0.9f) goUp = false;
	if (alpha <= 0.1f) goUp = true;
	
	if (goUp) alpha += incr;
	else alpha -= incr;
	
	this.gameObject.renderer.sharedMaterial.color.a = alpha;
}