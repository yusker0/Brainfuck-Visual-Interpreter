MAX = 32
step = 1
arr = [0]*MAX
ptr = 0
def readCode(code):
    global ptr
    ch = 0 
    while ch != len(code):
        match code[ch]:
            case '+':
                if arr[ptr] < 255:
                    arr[ptr] += 1
                else:
                    arr[ptr] = 0 
            case '-':
                if arr[ptr] > 0:
                    arr[ptr] -= 1
                else:
                    arr[ptr] = 255
            case '>':
                if ptr < MAX - 1:
                    ptr += 1
                else:
                    print(f'WARNING pointer must be < {MAX - 1}')
            case '<':
                if ptr > 0:
                    ptr -= 1
                else:
                    print('WARNING pointer must be >= 0')
            case '.':
                print(chr(arr[ptr]), end='')
            case ',':
                arr[ptr] = ord(input())
            case '[':
                close = findClose(code[ch+1::])
                if close == -1:
                    raise SyntaxError('Сycle not completed')
                while arr[ptr] != 0:
                    readCode(code[ch+1:ch+close+1])
                ch += close
        ch += 1     
def findClose(code):
    opens = 1
    closes = 0
    for ch in range(len(code)):
        if code[ch] == ']' and opens == closes+1:
            return ch
        elif code[ch] == ']':
            closes += 1
        elif code[ch] == '[':
            opens += 1
    return -1
