import { writable } from "svelte/store";
import log from "./log";

interface MercivMessage {
    event: string;
    args: any[];
    'rpc-id'?: number;
    'response-id'?: number;
}

class MercivUnityBridge {
    private rpcId = 0;
    private eventHandlers: { [event: string]: ((...args: any[]) => any)[] } = {};
    private rpcs: { [rpcId: number]: (returnValue: any) => void } = {};
    private window: Window | null = null;

    public started = writable(false);

    public setUnityWindow(window: Window) {
        log.verbose('JS setUnityWindow');
        this.window = window;
        this.rpcId = 1;
        this.eventHandlers = {};
        this.rpcs = {};
        this.window.addEventListener('merciv-unity-to-js', this.onUnityMessage.bind(this) as EventListener);
        this.started.set(true);
    }

    public async call<T>(event: string, ...args: any[]): Promise<T> {
        log.verbose('JS call', event);
        return new Promise<T>(resolve => {
            const rpcId = this.rpcId++;
            this.rpcs[rpcId] = resolve;
            this.send({ event, args, 'rpc-id': rpcId });
        });
    }

    public raiseEvent(event: string, ...args: any[]) {
        log.verbose('JS raiseEvent', event);
        this.send({ event, args });
    }

    public subscribe(event: string, handler: (...args: any[]) => any) {
        log.verbose('JS subscribe', event);
        if (!this.eventHandlers[event]) {
            this.eventHandlers[event] = [];
        }

        this.eventHandlers[event].push(handler);
    }

    public unsubscribe = (event: string, handler: (...args: any[]) => any) => {
        log.verbose('JS unsubscribe', event);
        this.eventHandlers[event] = this.eventHandlers[event].filter(h => h !== handler);
    }

    public createRpc<T>(event: string, handler: (...args: any[]) => any) {
        this.subscribe(event, handler);
    }

    public removeRpc = (event: string, handler: (...args: any[]) => any) => {
        this.unsubscribe(event, handler);
    }

    private send(message: MercivMessage) {
        log.verbose('JS send', message);
        const event = new CustomEvent('merciv-js-to-unity', { detail: JSON.stringify(message) });
        this.window?.dispatchEvent(event);
    }

    private onUnityMessage(event: CustomEvent<any>) { // Update the type of the event parameter
        const customEvent = event as CustomEvent<any>;
        log.verbose('JS onUnityMessage', customEvent.detail);

        const message = JSON.parse(customEvent.detail);

        if (message['response-id']) {
            log.verbose('JS onUnityMessage: response', message);
            const responseId = message['response-id'];
            const response = this.rpcs[responseId];
            if (!response) {
                log.verbose('JS onUnityMessage: no response handler for id', responseId);
                return;
            }

            response(message.args[0]);
            delete this.rpcs[responseId];
        } else {
            log.verbose('JS onUnityMessage: event', message);
            if (!this.eventHandlers[message.event]) {
                log.verbose('JS onUnityMessage: no handlers for event', message.event);
                return;
            }

            for (const handler of this.eventHandlers[message.event]) {
                const returnValue = handler(...message.args);
                if (message['rpc-id']) {
                    this.send({ event: message.event, args: [returnValue], 'response-id': message['rpc-id'] });
                }
            }
        }
    }
}

const unity = new MercivUnityBridge();
export default unity;