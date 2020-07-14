import sys
import cx_Oracle

def loadOrderList(inputFile):
    orders = []
 
    listFile = open(inputFile, 'r')

    while 1:
        line = listFile.readline().strip()

        if not line:
            break

        orders.append(line)

    listFile.close()
    return orders

def loadTemplate(inputFile):
    templateLines = []
 
    sqlFile = open(inputFile, 'r')

    while 1:
        line = sqlFile.readline()

        if not line:
            break

        templateLines.append(line)

    sqlFile.close()
    return templateLines

def getOrders(query, tnsname, userId, password):
    conn = cx_Oracle.connect(userId, password, tnsname)
    curs = conn.cursor()
    orders = []

    if curs.execute(query):
        for row in curs:
            orders.append(row[0])

    conn.close()
    return orders 

def createUpdateQuery(orders, templateLines, outputQueryFile):
    # Write Transfer List
    count=0
    orderList="";
    f = open(outputQueryFile, 'w')

    for order in orders:
        count+=1

        if (count > 1):
            orderList+=","
            orderList+=str(order)
        else:
            orderList+=str(order)
        
    for i in range(0, 5):
        f.write(templateLines[i])

    f.write(templateLines[5].format(orderList))
        
    for i in range(6, 9):
        f.write(templateLines[i])

    for order in orders:
        for i in range(9, 17):
            f.write(templateLines[i])

        f.write(templateLines[17].format(order))

        for i in range(18, 22):
            f.write(templateLines[i])

        f.write(templateLines[22].format(order))
        f.write(templateLines[23])

    for i in range(24, 30):
        f.write(templateLines[i])

    f.write(templateLines[30].format(orderList))

    for i in range(31, 37):
        f.write(templateLines[i])

    f.write(templateLines[37].format(orderList))

    for i in range(38, 47):
        f.write(templateLines[i])

    f.write(templateLines[47].format(orderList))

    for i in range(48, 53):
        f.write(templateLines[i])

    f.write(templateLines[53].format(orderList))

    for i in range(54, 64):
        f.write(templateLines[i])

    f.write(templateLines[64].format(orderList))
    f.write(templateLines[65])
    f.close()
    return

def main():
    if len(sys.argv) < 3:
        print('Usage: SettleCpsOrders.py [Order List Filename] [Update Query Filename]')
        orderListFile = 'orders.txt'
        outputQueryfile = 'UpdateQuery.sql'
    else:
        orderListFile =  sys.argv[1]
        outputQueryfile = sys.argv[2]
           
    orders = loadOrderList(orderListFile)
    templateLines = loadTemplate('.\sql\\updateTemplate.sql')
    createUpdateQuery(orders, templateLines, outputQueryfile)
    return
    
main()