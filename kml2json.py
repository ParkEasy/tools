from xml.dom import minidom
import json

f = open("doc.kml")

s = f.read()
s = s
xmldoc = minidom.parseString(s)
f.close()

itemlist = xmldoc.getElementsByTagName("Placemark")

print str(len(itemlist)) + " Placemarks"

results = []
for s in itemlist:

	coords = s.getElementsByTagName("Point")[0].getElementsByTagName("coordinates")[0].firstChild.nodeValue
	desc = s.getElementsByTagName("description")[0].firstChild.nodeValue

	print desc

	item = {
		"name": s.getElementsByTagName("name")[0].firstChild.nodeValue,
		"coordinates": {
			"latitude": float(coords.split(",")[1]),
			"longitude": float(coords.split(",")[0])
		},
		"descriptions": desc.split("<br>")
	} 

	results.append(item)

print results

f = open("doc.json", "w")
f.write(json.dumps(results, indent=4, separators=(",", ": ")))
f.close()

print "Done. Check data.json"