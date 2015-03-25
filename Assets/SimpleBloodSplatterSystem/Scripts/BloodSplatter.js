#pragma strict
//Script written by Deatrocker
//deatrocker.ucoz.com
//Thanx for using! you can delete this comment
//
//Intro: This script is attached to every blood chunk what instantiated from enemy with getting damage. When chunk hit object with allowed tag,
//it instantiate splat and self destroying. Good Luck!

var BloodSplatterPrefabSmall : Transform; //Prefab of our splat
var SplatterMaterials : Material[];	//All materials what will be randomly choosed every instantiation for our splat prefab
var ChunkSpeed:float=10000; //speed of chunk
var RandomSizeFrom: float=0.3; //random size of splat
var RandomSizeTo:float=1.0;    //random size of splat
var TimeToLive:float=1.0;		//If chunk dont hit some object with allowed tag in timetolive, it will be destroyed
var AllowedTags:String[];		//for example we dont need to instantiate splat on weapons, pens and etc, so we can awoid it and set allowed tags for splat.
var duration : float = 1.5f;
private var Size:float;  //dont get in mind , just for system uses


function Start(){
gameObject.GetComponent.<Rigidbody>().AddForce (Random.Range(-ChunkSpeed,ChunkSpeed), Random.Range(-ChunkSpeed,ChunkSpeed), Random.Range(-ChunkSpeed,ChunkSpeed)); //Adding random force to each chunk for realizm
Destroy(gameObject,TimeToLive); //Start timer to destroy
}

function Update(){
	duration-=Time.deltaTime;
	if(duration<=0){
		//Destroy(gameObject);
	}
}

function OnCollisionEnter (col: Collision){
for(var item : String in AllowedTags){ //It checks all our allowed tags
if (col.gameObject.CompareTag(item)) //if chunk hit something with allowed tag 
	{
		Size=Random.Range(RandomSizeFrom,RandomSizeTo); //set random size of splat
		BloodSplatterPrefabSmall.localScale= Vector3(Size,Size,Size); //yea
		var splatMaterial = Random.Range(0, SplatterMaterials.length); //set random material from array of our splat mats
		var contact : ContactPoint; //emmm...dont get in mind its a contact point, where our splat is
		contact = col.contacts[0]; //dont get in mind it should be here
	  	var rot = Quaternion.FromToRotation(Vector3.up, contact.normal); // calculate rotation from prefab normal to contact.normal
	    var Splat = Instantiate(BloodSplatterPrefabSmall, contact.point, rot); //Instantiation of our splat depends on contact point and its normal and rotation
	   	Splat.GetComponent.<Renderer>().material = SplatterMaterials[splatMaterial]; //apply random material to splat
	    Splat.parent = col.transform; // child the hole to the hit object
	    Destroy(gameObject); //destroy blood chunk
	}
}
} 


