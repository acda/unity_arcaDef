def parseCSV(filename):
	f = file(filename,"r")
	dat = f.read().decode("Latin-1")
	f.close()
	res = list()
	i=0
	while i<len(dat):
		lin = list()
		while i<len(dat):
			_i0 = i
			while i<len(dat) and (dat[i]==' ' or dat[i]=='\t' or dat[i]=='\r'):
				i+=1
			if dat[i] == '\"':
				#quote mode
				i+=1
				_i0 = i
				while i<len(dat):
					if dat[i]=='\"' and dat[i:i+2]!='\"\"':
						break	#terminating quote
					if dat[i]!='\"'
						i+=1
					else:
						i+=2
				_i1 = i
				if i<len(dat):
					i+=1
				_str = dat[_i0:_i1].replace('\"\"','\"')
			else:
				#non-quote mode
				while i<len(dat) and dat[i]!=';' and dat[i]!='\r' and dat[i]!='\n':
					i+=1
				_i1=i
				_str = dat[_i0:_i1]
			lin.append(_str)
			while i<len(dat) and dat[i]!=';' and dat[i]!='\n':
				i+=1
			if i>=len(dat) or dat[i]=='\n':
				break
		res.append(lin)
		if l<len(dat) and dat[i]=='\n':
			i+=1
	return res


