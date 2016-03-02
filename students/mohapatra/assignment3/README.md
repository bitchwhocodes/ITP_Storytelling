# STUDENT

Sweta Mohapatra

## Assignment Number 3

Rain on Me - First Version

Process: 

<ul>
  <li>Took the Kinect examples as baseline</li>
  <li>In the spaceship example, disable the ship object</li>
  <li>Copy over the bodyparticles object from the BodyParticles scene</li>
  <li>Add RigidBody and Box Collider component to BodyParticle object (same components as were on the original ship object)</li>
  <li>Duplicated the Cube prefab and changed X and Y scale to 1, Z scale to .5. Called it SkeletonCube</li>
  <li>Check to make sure that BodyParticles and ObjectDrop are on the same Z axis - Position: Z - 10</li>
  <li>Replaced the Joints variable from ParticleSystem to SkeletonCube</li>
  <li>In BodyParticles.cs, commented out the Particle System Code and added OnCollisionEnter to detect when cubes dropping down collide with the skeleton</li>
</ul>
