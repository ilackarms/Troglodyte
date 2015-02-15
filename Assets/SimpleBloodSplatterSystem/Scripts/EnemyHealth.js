#pragma strict
//Script written by Deatrocker
//deatrocker.ucoz.com
//Thanx for using! you can delete this comments
//
//Intro: This script is a blank of enemy in your game, just implement it in your game with structure saving
//for example copy Start() fuction content in your enemy Start() function etc. Dont forget to implement instantiation
//of blood chunks to your damage system, cos here its a blank with just mouse click. Good luck!

var MaxHP:float=100; //Maximum health
var HP:float;       //Current health
var Invincible:boolean;
var BloodChunk: GameObject[]; //array of blood drops prefabs
var PositionOfBlood:GameObject[]; //array of positions(empty game objects) where blood will be instantiated
var BloodAmount:float=0.5; // Amount of dropped bloodchunks per one hit. Be carefull ^.^ dont set > 30
var DeadPrefab:Rigidbody; //Prefab of dead enemy
private var coun:int;

function Start () {
HP=MaxHP; //In start our enemy must be healthy
}

function Update () {
if(Input.GetMouseButton(0)) Damage(Random.Range(1.0,4.0)); //Call function Damage with damage parameter
}

function Damage(dam:float)
{
	if(HP<1.0) return; //if enemy dead - dont execute next
	if (!Invincible) HP-=dam;  //if enemy still alive then get damage
	for(var i : int = 0; i <= BloodAmount; i++){ //instantiate as much blood drops as you set in BloodAmount variable
	var bloodChunk=Instantiate(BloodChunk[Random.Range(0,BloodChunk.length)],PositionOfBlood[Random.Range(0,PositionOfBlood.length)].transform.position,gameObject.transform.rotation);//instantiation
	}
	if(HP<=1.0){ //if enemy are theoretically dead
	var deadEnemy=Instantiate(DeadPrefab,transform.position,transform.rotation); // lets made it dead! =)
	Destroy(gameObject); //Destroy enemy cos we instantiated his dead body
	}
}