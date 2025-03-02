from collections import defaultdict

class PrototypeTableProcessor:
    def __init__(self):
        """
        self.table = {
            "key": {
                "english(en)": "Hello",
                "korean(ko)": "안녕"
            }
        }
        """
        self.table = defaultdict(lambda : defaultdict(str))

        """
        self.locales = ["english(en)", "korean(ko)"]
        """
        self.locales = []

    @staticmethod
    def _to_pascal(content):
        return content.title().replace(" ", "")
    _p = _to_pascal

    @staticmethod
    def _to_pascal_row(row):
        return [*map(lambda each: each.title().replace(" ", ""), row)]
    _pr = _to_pascal_row

    def feed(self, table):
        header = [*map(lambda each: each.lower(), table[0])]
        if "key" not in header:
            raise ValueError("Key column is missing. It seems like the table is not containing the header row.")
        
        for locale in header[1:]:
            if locale not in self.locales:
                self.locales.append(locale)
        
        for row in table[1:]:
            key = row[0]
            for i in range(1, len(row)):
                self.table[key][header[i]] = row[i]
    
    def render(self):
        body = []
        body.append(PrototypeTableProcessor._pr(
            ["key", *self.locales]
        ))
        
        for key, values in self.table.items():
            body.append(PrototypeTableProcessor._pr(
                [key, *values.values()]
            ))

        return body
