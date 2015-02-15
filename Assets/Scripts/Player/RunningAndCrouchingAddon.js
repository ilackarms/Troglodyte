#pragma strict

 var walkSpeed: float; // regular speed
 var crchSpeed: float; // crouching speed
 //var runSpeed: float = 20; // run speed
 

private var chMotor: CharacterMotor; private var ch: 
CharacterController; private var tr: Transform; 
private var height: float; // initial height

function Start(){ 
	chMotor = GetComponent(CharacterMotor); 
	tr = transform; ch = GetComponent(CharacterController); height = ch.height; 
	walkSpeed = chMotor.movement.maxForwardSpeed;
	crchSpeed = walkSpeed * .035;
}

function Update(){var h = height;
 
 var speed = walkSpeed;
 
 /*if (ch.isGrounded && Input.GetButton("Run")){
   //  speed = runSpeed;
 }
 */
 
 if (Input.GetButton("Crouch")){ // press C to crouch
     h = 0.35 * height;
     //if(chMotor.grounded) speed = crchSpeed; // slow down when crouching
 }
 //chMotor.movement.maxForwardSpeed = speed; 
 //chMotor.movement.maxBackwardsSpeed = speed; 
 //chMotor.movement.maxSidewaysSpeed = speed; // set max speed  
 var lastHeight = ch.height; // crouch/stand up smoothly 
 ch.height = Mathf.Lerp(ch.height, h, 5*Time.deltaTime);
 tr.position.y += (ch.height-lastHeight)/2; // fix vertical position
 }