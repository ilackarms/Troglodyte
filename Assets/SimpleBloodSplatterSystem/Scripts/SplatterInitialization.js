#pragma strict
//Script written by Deatrocker
//deatrocker.ucoz.com
//Thanx for using! you can delete this comments
//
//Intro: This script is attached to prefab of plane what will be our blood splat. It controls fading in of instantiation,
//fading out, type of destruction and its full of logic =) Good luck!

var fadeInTime: float=1.0; //fade in of blood splat
var fadeOutTime: float = 0.05; //fade out of blood splat
var SmartDestroy: boolean=false; //SD - splat will be automatically destroyed after fadingout
var ManualDestroyTime:float=5; //Time to manual destroy
private var fadeout:boolean; //helping var

function Start(){
transform.localRotation.eulerAngles.y = Random.value * 360; //rotate our splat in random
renderer.material.color.a=0; //start with no visible for fade in feature
fadeout=false;
if(!SmartDestroy) Destroy(gameObject,ManualDestroyTime); //if SD unchecked, than start timer of manual destroy
}

function Update () {
	if(renderer.material.color.a<1.0 && !fadeout) { //Fading in
		renderer.material.color.a +=Time.deltaTime*fadeInTime;
	} else {
		fadeout=true; //activation of fading out
		if(fadeout && renderer.material.color.a<=0.0 && SmartDestroy){ //if SD and our splat become dissapeared - destroy go
			Destroy(gameObject);
		}
		if(fadeout) renderer.material.color.a -= Time.deltaTime *fadeOutTime; //if its still not destroyed fading out
	}
}