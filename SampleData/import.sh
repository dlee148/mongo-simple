mongo test --eval "db.docs.drop()"

for file in *.json; do
	mongoimport --db test --collection docs --file $file
done
