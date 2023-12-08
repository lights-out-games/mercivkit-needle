import { Behaviour, serializable, Animator } from "@needle-tools/engine"
import { Object3D, Vector3 } from "three"

export class Player extends Behaviour {

    @serializable(Object3D)
    camera?: Object3D;

    @serializable()
    speed: number = 0.1;

    @serializable()
    rotateSpeed: number = 0.1;

    @serializable(Vector3)
    cameraOffset: Vector3 = new Vector3(0, 2, -2);

    @serializable(Animator)
    animator?: Animator;

    start() {
    }

    update() {
        let moving = false;

        if (this.context.input.isKeyPressed("w")) {
            this.gameObject.position.add(this.gameObject.worldForward.multiplyScalar(this.speed));
            moving = true;
        }

        if (this.context.input.isKeyPressed("s")) {
            this.gameObject.position.add(this.gameObject.worldForward.multiplyScalar(-this.speed));
            moving = true;
        }

        this.animator?.setFloat("MoveSpeed", moving ? 1 : 0);

        if (this.context.input.isKeyPressed("a")) {
            this.gameObject.rotateY(this.rotateSpeed);
        }

        if (this.context.input.isKeyPressed("d")) {
            this.gameObject.rotateY(-this.rotateSpeed);
        }

        // const offset = this.cameraOffset.clone();
        // offset.applyQuaternion(this.gameObject.quaternion);
        // const position = this.gameObject.position.clone();
        // position.add(offset);
        // this.camera?.position.copy(position);
        this.camera?.lookAt(this.gameObject.position);
    }
}