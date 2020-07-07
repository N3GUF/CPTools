import sys
import cx_Oracle

def loadQuery(inputFile):
    sql = ''
 
    sqlFile = open(inputFile, 'r')

    while 1:
        line = sqlFile.readline().strip()

        if not line:
            break

        sql += line + ' '

    sqlFile.close()
    return sql

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
    f.write(templateLines[0])
    f.write(templateLines[1])
    f.write(templateLines[2])

    for order in orders:
        count+=1

        if (count > 1):
            orderList+=","
            orderList+=str(order)
        else:
            orderList+=str(order)

        f.write(templateLines[3])
        f.write(templateLines[4])
        f.write(templateLines[5])
        f.write(templateLines[6])
        f.write(templateLines[7])
        f.write(templateLines[8])
        f.write(templateLines[9])
        f.write(templateLines[10])
        f.write(templateLines[11])
        f.write(templateLines[12].format(order))
        f.write(templateLines[13])
        f.write(templateLines[14])
        f.write(templateLines[15])
        f.write(templateLines[16])
        f.write(templateLines[17])
        f.write(templateLines[18])
        f.write(templateLines[19])
        f.write(templateLines[20].format(order))
        f.write(templateLines[21])
        f.write(templateLines[22])
        f.write(templateLines[23].format(order))
        f.write(templateLines[24])

    f.write(templateLines[25])
    f.write(templateLines[26])
    f.write(templateLines[27])
    f.write(templateLines[28])
    f.write(templateLines[29])
    f.write(templateLines[30])
    f.write(templateLines[31])
    f.write(templateLines[32])
    f.write(templateLines[33].format(orderList))
    f.close()
    return

def main():
    if len(sys.argv) < 2:
        print('Usage: PilotCarBillingFix.py [Update Query Filename]')
        outputQueryfile = 'PilotCarBillingUpdateQuery.sql'
    else:
        outputQueryfile = sys.argv[1]
           
    selectQuery = loadQuery('.\sql\PilotCarOrdersNotBilled.sql')
    templateLines = loadTemplate('.\sql\\updateTemplate.sql')
    orders = getOrders(selectQuery, 'ngcprd', 'dbernhardy','?Camer0n2')
    createUpdateQuery(orders, templateLines, outputQueryfile)
    return
    
main()