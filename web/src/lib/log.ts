// const VERBOSE_LOG = false
const VERBOSE_LOG = true

class Log {
    public verbose(...args: any[]) {
        if (VERBOSE_LOG) {
            console.log('SV:', ...args);
        }
    }

    public info(...args: any[]) {
        console.info('SV:', ...args);
    }

    public warn(...args: any[]) {
        console.warn('SV:', ...args);
    }

    public error(...args: any[]) {
        console.error('SV:', ...args);
    }
}

const log = new Log();
export default log;