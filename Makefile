BIN = bin/debug
PUB = bin/publish

build:
	@mkdir -p $(BIN)
	dotnet build --output $(BIN)
	@echo --------------------------------

run:
	@dotnet ./$(BIN)/ptk.dll

test:
	@mkdir -p $(BIN)
	dotnet build --output $(BIN)
	@echo --------------------------------
	@dotnet ./$(BIN)/ptk.dll

clean:
	@rm -rf ./$(BIN)
