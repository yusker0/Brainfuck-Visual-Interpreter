import sys
import interpretator as intp


try:
    if len(sys.argv) > 0:
        f = open(sys.argv[1], 'r')
    else:
        raise AttributeError('Please enter filename')   
    if not (sys.argv[1].split('.')[-1] == 'bf' or sys.argv[1].split('.')[-1] == 'b'):
        raise FileNotFoundError('File must be with extension .b or .bf')
    code = f.read()
    intp.readCode(code)
except FileNotFoundError:
    raise FileNotFoundError('File not found')