#!/usr/bin/python
# -*- coding: latin-1 -*-

"""
 Plan für neues Dateiformat
 Oben: Zeilen, Frz | deutsch
 Unten: hash der frz. wörter | rang | historie
"""

import sys
import math
import time
import random


RANDOMIZE_ALL = False

ORGNAME = ".\\Fr.txt"
TMPNAME = ".\\Fr_t.txt"


class item(object):
	__slots__ = ('franc','explic','hist')
	def __init__(self,f,e):
		self.franc = f
		self.explic = e
		self.hist = ''

	def __repr__(self):
		return "item(%s,%s)" % (repr(self.franc),repr(self.explic))


def parseFile(filename):
	global RANDOMIZE_ALL
	try:
		f = file(filename,"r")
		lf = set()
		lineCount = 0
		slist = list()
		for lin in f:
			lineCount += 1
			lin = lin.rstrip()
			if lin.strip()[:1]=='#' or lin.find('|')<0:
				continue
			_parts = lin.split('|')
			if len(_parts)<2 or len(_parts)>4:
				sys.stderr.write("Line %u should have 4 parts. It has %d: \"%s\"\n",lineCount,len(_parts),lin)
				continue
			while len(_parts)<4:
				_parts.append('')
			_fr = _parts[0].strip().decode("Latin-1")
			_ex = _parts[1].strip().decode("Latin-1")
			_rank = _parts[2].strip()
			if _rank == '':
				_rank = '-1'
			try:
				_rank = int(_rank)
			except ValueError,er:
				sys.stderr.write("Line %u: Rank not an integer in line.\n",lineCount)
				continue

			if RANDOMIZE_ALL:
				_rank = random.randint(0,0x000FFFFF)


			_hist = _parts[3].strip()
			if _fr in lf:
				sys.stderr.write("duplicate item '%s'\n"%(_fr.encode("Latin-1")))
			else:
				lf.add(_fr)
				_it = item( _fr , _ex )
				_it.hist = _hist
				slist.append(( _rank , _it ))
		f.close()
	except IOError,ex:
		return None
	slist.sort()
	items = list()
	for _dummy,_it in slist:
		items.append(_it)
	return items


def writeFile(filename,itemList):
	print "calling write!!!";
	f = file(filename,"w")
	ALIGN_FR=36
	ALIGN_EX=72
	slist = list()
	_rank = 0
	for item in itemList:
		slist.append((item.franc,_rank,item))
		_rank += 1
	slist.sort()
	for _dummy1,_rank,item in slist:
		_fr = item.franc.encode("Latin-1")
		_ex = item.explic.encode("Latin-1")
		_fr = padWithTabs(_fr,0,ALIGN_FR)
		_ex = padWithTabs(_ex,1,ALIGN_EX)
		lin = _fr+'|'+_ex+'|'+("%4d"%_rank)+'|'+item.hist+"\n"
		f.write(lin)
	f.close()

def keyfunc(i):
	return i.franc + "   ###   " + i.explic

def doit(items):
	while True:
		# show first item.
		itm = items[0]
		print "" + itm.franc
		showed = False
		while True:
			r = sys.stdin.readline()
			if (r is None) or (len(r)<1):
				return
			r = r.strip()
			cer = 0
			if r=="":
				print "  -> "+itm.explic.encode("CP437")
				showed = True
				continue	# don't know input
			try:
				cer = int(r.strip())
				if cer<0 or cer>9:
					sys.stderr.write("Bad input. Outside of range 1..9.")
					cer = None
			except ValueError,er:
				sys.stderr.write("Bad input. Not an integer.")
				cer = None
			if cer is not None:
				break
		# cer is now None (dunno), or 1-9 for certainty. 9 is Sure.
		if not showed:
			print "  -> "+itm.explic.encode("CP437")
		val = newPos(cer/9.0,len(items))
		_cer = ('%u'%int(cer))[-1]
		itm.hist = (itm.hist+_cer)[-20:]
		del items[0]
		#print "   move down to %d" % val
		items.insert(val,itm)

def newPos(cer,llen):
	# cer is a value from 0 to 1.
	val = (cer-1.0)*random.gauss(1,0.04)
	if val>0.0:
		val=0.0
	_efac = 3.15
	val = math.exp(val*_efac)		# value from some small value up to 1.0
	val = int((llen-1)*val+0.5)
	if val<1: val=1
	return val


def mergeLists(mainList,addIn):
	_is = set()
	for i in mainList:
		_is.add(i.franc)
	inpoint = 0
	for i in addIn:
		if i.franc not in _is:
			print "inserting item " + repr(i.franc) + "at %u"%inpoint
			_is.add(i.franc)
			mainList.insert(inpoint,i)
			inpoint += 1
	return mainList


def warnSimilars(lis):
	s = dict()
	for itm in lis:
		nn = stripMin(itm.franc)
		if nn not in s:
			s[nn] = itm
		else:
			print "  DUPLICATE?"
			print "      " + repr(itm)
			print "      " + repr(s[nn])

def stripMin(s):
	s = s.strip().replace(' ','#').replace('.','#').replace(',','#').replace('\'','#').replace('%','#').replace('%','#').replace(':','#').replace('-','#').replace(';','#').replace('(','#').replace(')','#')
	s = s.split('#')
	res = s[0]
	for i in s:
		if len(i)>len(res) and (i!='adj'):
			res = i
	return res

def padWithTabs(text,headOffset,desiredLen):
	l = len(text)
	desiredLen = ((headOffset+desiredLen+3)&0x7FFFFFFC)-headOffset
	if l<desiredLen:
		text = text + ('\t'*((desiredLen+3-l)//4))
	return text

def debugDump(i,maxlen):
	t=0
	for t in xrange(min(len(i),maxlen)):
		it = i[t]
		print "%-30s|%s" % (it.franc,it.explic)



def main(args):
	global ORGNAME
	global TMPNAME
	i = parseFile(TMPNAME)
	io = parseFile(ORGNAME)
	if i is None:
		i = io
	elif io is not None:
		pass # i = mergeLists(i,io)
	if io is not None:
		del io
	if i is None:
		sys.stderr.write("Cannot read vocabulary file\n");
		return 5;

	for itm in args:
		i2 = parseFile(itm)
		if i2 is None:
			sys.stderr.write("cannot process additional file '%s'\n"%itm)
			return 5
		i = mergeLists(i,i2)
		del i2

	print "loaded. Have %u items" % len(i)

	warnSimilars(i)

	print "type certainty. 0==dunno, 1==weak, 9==absolutely sure."

	print "positions for certainty 0..9 : "+'  '.join(("%d"%newPos(ii/9.0,len(i))) for ii in xrange(0,10))

	#debugDump(i,30)

	doit(i)

	time.sleep(0.5)
	writeFile(TMPNAME,i)


	for item in i:
		print repr(item).encode("CP437")









if __name__=="__main__":
	sys.exit(main(sys.argv[1:]))

